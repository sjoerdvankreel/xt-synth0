#include <DSP/Synth/PlotDSP.hpp>
#include <DSP/Synth/ModDSP.hpp>
#include <DSP/Synth/AmpDSP.hpp>
#include <DSP/Synth/EnvDSP.hpp>
#include <DSP/Synth/LfoDSP.hpp>
#include <DSP/Synth/UnitDSP.hpp>
#include <DSP/Synth/FilterDSP.hpp>
#include <DSP/SynthDSP.hpp>
#include <DSP/Utility.hpp>

#include <iomanip>
#include <sstream>

namespace Xts {

// https://stackoverflow.com/questions/604453/analyze-audio-using-fast-fourier-transform
// https://dsp.stackexchange.com/questions/46692/calculating-1-3-octave-spectrum-from-fft-dft
static float
Power(std::vector<std::complex<float>>& fft, float rate, int oct, int note)
{
  float result = 0.0f;
  float freq2Bin = rate / (fft.size() * 2.0f);
  float midi = static_cast<float>(oct * 12 + note);
  size_t bin1 = static_cast<size_t>(MidiNoteFrequency(midi) * freq2Bin);
  size_t bin2 = static_cast<size_t>(MidiNoteFrequency(midi + 1) * freq2Bin);
  for (size_t i = bin1; i < bin2 && i < fft.size(); i++)
    result += fft[i].real() * fft[i].real() + fft[i].imag() * fft[i].imag();
  result = std::sqrtf(result);
  assert(!std::isnan(result));
  return result;
}

static void
Fft(std::complex<float>* x, std::complex<float>* scratch, size_t count)
{
  assert(count == NextPowerOf2(count));
  if (count < 2) return;
  std::complex<float>* even = scratch;
  std::complex<float>* odd = scratch + count / 2;
  for (size_t i = 0; i < count / 2; i++) even[i] = x[i * 2];
  for (size_t i = 0; i < count / 2; i++) odd[i] = x[i * 2 + 1];
  Fft(odd, x, count / 2);
  Fft(even, x, count / 2);
  for (size_t i = 0; i < count / 2; i++)
  {
    float im = -2.0f * PIF * i / count;
    std::complex<float> t = std::polar(1.0f, im) * odd[i];
    x[i] = even[i] + t;
    x[i + count / 2] = even[i] - t;
  }
}

static void
Fft(std::vector<float> const& x, std::vector<std::complex<float>>& fft, std::vector<std::complex<float>>& scratch)
{
  assert(x.size() == NextPowerOf2(x.size()));
  fft.resize(x.size());
  scratch.resize(x.size());
  for (size_t i = 0; i < x.size(); i++)
    fft[i] = std::complex<float>(x[i], 0.0f);
  Fft(fft.data(), scratch.data(), x.size());
}

static void
Spectrum(
  std::vector<float>& x, 
  std::vector<std::complex<float>>& fft, 
  std::vector<std::complex<float>>& fftScratch,
  float rate)
{
  float max = 0;
  assert(x.size() > 0 && x.size() == NextPowerOf2(x.size()));
  Fft(x, fft, fftScratch);
  x.clear();
  fft.erase(fft.begin() + fft.size() / 2, fft.end());  
  for(int oct = 0; oct < 12; oct++)
  {
    for(int note = 0; note < 12; note++)
      x.push_back(Power(fft, rate, oct, note));
  }    
  for (size_t i = 0; i < x.size(); i++) max = std::max(x[i], max);
  for (size_t i = 0; i < x.size(); i++) x[i] = max == 0.0f? 0.0f: x[i] / max;
}

template <class T>
static int
GroupIndex(PlotType type, T base)
{ return static_cast<int>(type) - static_cast<int>(base); }

static void
RenderLfo(SynthModel const& model, PlotInput const& input, PlotOutput& output)
{
  LfoPlotState state;
  state.input = &input;
  state.output = &output;
  state.spectrum = model.plot.spec;
  state.model = &model.cv.lfos[GroupIndex(model.plot.type, PlotType::LFO1)];
  LfoDSP::Plot(&state);
}

static void
RenderUnit(SynthModel const& model, PlotInput const& input, PlotOutput& output)
{
  UnitPlotState state;
  state.cv = &model.cv;
  state.input = &input;
  state.output = &output;
  state.spectrum = model.plot.spec;
  state.model = &model.audio.units[GroupIndex(model.plot.type, PlotType::Unit1)];
  UnitDSP::Plot(&state);
}

static void
RenderFilter(SynthModel const& model, PlotInput const& input, PlotOutput& output)
{
  FilterPlotState state;
  state.cv = &model.cv;
  state.input = &input;
  state.output = &output;
  state.audio = &model.audio;
  state.spectrum = model.plot.spec;
  state.index = GroupIndex(model.plot.type, PlotType::Filt1);
  state.model = &model.audio.filters[state.index];
  FilterDSP::Plot(&state);
}

static void
RenderEnv(SynthModel const& model, int hold, PlotInput const& input, PlotOutput& output)
{
  EnvPlotState state;
  state.hold = hold;
  state.input = &input;
  state.output = &output;
  state.model = &model.cv.envs[GroupIndex(model.plot.type, PlotType::Env1)];
  EnvDSP::Plot(&state);
}

static void
RenderAmp(SynthModel const& model, int hold, PlotInput const& input, PlotOutput& output)
{
  AmpPlotState state;
  state.hold = hold;
  state.cv = &model.cv;
  state.input = &input;
  state.output = &output;
  state.model = &model.amp;
  state.env = &model.cv.envs[static_cast<int>(model.amp.ampEnvSource)];
  AmpDSP::Plot(&state);
}

void
PlotDSP::Render(SynthModel const& model, PlotInput const& input, PlotOutput& output)
{
  auto type = model.plot.type;
  auto index = static_cast<int>(type);
  int ampEnv = static_cast<int>(model.amp.ampEnvSource);
  EnvModel const& envModel = model.cv.envs[ampEnv];
  int hold = model.plot.spec && (model.plot.type == PlotType::Synth || model.plot.type >= PlotType::LFO1)? SpecHold: model.plot.hold;

  switch(model.plot.type)
  {
  case PlotType::Synth: {
    SynthDSP::Plot(model, envModel, model.plot.spec, hold, input, output);
    break; }
  case PlotType::Amp: RenderAmp(model, hold, input, output); break;
  case PlotType::LFO1: case PlotType::LFO2: case PlotType::LFO3: RenderLfo(model, input, output); break;
  case PlotType::Unit1: case PlotType::Unit2: case PlotType::Unit3: RenderUnit(model, input, output); break;
  case PlotType::Filt1: case PlotType::Filt2: case PlotType::Filt3: RenderFilter(model, input, output); break;
  case PlotType::Env1: case PlotType::Env2: case PlotType::Env3: RenderEnv(model, hold, input, output); break;
  default: assert(false); break;
  }  
  
  assert(output.rate <= input.rate);  
  if(!output.spectrum) return;  
  output.min = 0.0f;
  output.max = 1.0f;

  if (output.lSamples->empty()) return;  
  output.lSamples->resize(NextPowerOf2(output.lSamples->size()));
  assert(output.lSamples->size() >= static_cast<size_t>(output.rate));
  Spectrum(*output.lSamples, *output.fftData, *output.fftScratch, output.rate);
  //SpectrumHSplits(*output.hSplits);
  
  if (!output.stereo)
  {
    //SpectrumVSplitsMono(*output.vSplits);
    return;
  }
  //SpectrumVSplitsStereo(*output.vSplits);
  output.rSamples->resize(NextPowerOf2(output.rSamples->size()));
  Spectrum(*output.rSamples, *output.fftData, *output.fftScratch, output.rate);
}



} // namespace Xts