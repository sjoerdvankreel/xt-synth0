#include "DSP.hpp"
#include "AmpDSP.hpp"
#include "EnvDSP.hpp"
#include "UnitDSP.hpp"
#include "PlotDSP.hpp"
#include "SynthDSP.hpp"
#include <DSP/Utility.hpp>

#include <vector>
#include <complex>
#include <cassert>
#include <iomanip>
#include <sstream>

namespace Xts {

static std::wstring
VSplitMarker(float val, float max)
{
  float absval = std::fabs(val);
  std::wstring result = val == 0.0f ? L"" : val > 0.0f ? L"+" : L"-";
  if(max >= 10)
  { 
    int ival = static_cast<int>(std::roundf(absval));
    return result + std::to_wstring(ival);
  }
  std::wstringstream str;
  str << std::fixed << std::setprecision(1) << absval;
  return result + str.str();
}

std::vector<VSplit> 
PlotDSP::BiVSPlits = {
  { -1.0f, L"+1.0" },
  { -0.5f, L"+0.5" },
  { 0.0f, L"0.0" },
  { 0.5f, L"-0.5" },
  { 1.0f, L"-1.0" }
};

std::vector<VSplit> 
PlotDSP::UniVSPlits = {
  { 0.0f, L"1.0" },
  { 0.25f, L".75" },
  { 0.5f, L"0.5" },
  { 0.75f, L".25" },
  { 1.0f, L"0.0" }
};

std::vector<VSplit>
PlotDSP::StereoVSPlits = {
  { -1.0f, L"+1.0" },
  { -0.5f, L"L" },
  { 0.0f, L"-/+1" },
  { 0.5f, L"R" },
  { 1.0f, L"-1.0" }
};

std::vector<VSplit> 
PlotDSP::MakeBiVSplits(float max)
{
  std::vector<VSplit> result;
  result.emplace_back(-1.0f, VSplitMarker(max, max));
  result.emplace_back(-0.5f, VSplitMarker(max / 2.0f, max));
  result.emplace_back(-0.0f, L"0");
  result.emplace_back(0.5f, VSplitMarker(-max / 2.0f, max));
  result.emplace_back(1.0f, VSplitMarker(-max, max));
  return result;
}

std::wstring
PlotDSP::FormatEnv(EnvelopeStage stage)
{
  switch (stage)
  {
  case EnvelopeStage::Attack: return L"A";
  case EnvelopeStage::Decay: return L"D";
  case EnvelopeStage::Sustain: return L"S";
  case EnvelopeStage::Release: return L"R";
  case EnvelopeStage::Delay: return L"D";
  case EnvelopeStage::Hold: return L"H";
  case EnvelopeStage::End: return L"";
  default: assert(false); return L"";
  }
}

static void
SpectrumHSplits(std::vector<HSplit>& hSplits)
{
  hSplits.clear();
  for (int oct = 0; oct < 12; oct++)
  {
    std::wstring marker = oct >= 2 ? std::to_wstring(oct - 2) : L"";
    hSplits.emplace_back(HSplit(oct * 12, marker));
  }
  hSplits.emplace_back(HSplit(143, L""));
}

static void
SpectrumVSplitsMono(std::vector<VSplit>& vSplits)
{
  vSplits.clear();
  vSplits.emplace_back(1.0f - (1.0f / 1.0f), L"1.0");
  vSplits.emplace_back(1.0f - (1.0f / 2.0f), L".50");
  vSplits.emplace_back(1.0f - (1.0f / 4.0f), L".25");
  vSplits.emplace_back(1.0f - (1.0f / 8.0f), L"");
  vSplits.emplace_back(1.0f - (1.0f / 16.0f), L"");
  vSplits.emplace_back(1.0f - (1.0f / 32.0f), L"");
  vSplits.emplace_back(1.0f, L"0.0");
}

static void
SpectrumVSplitsStereo(std::vector<VSplit>& vSplits)
{
  vSplits.clear();
  vSplits.emplace_back(1.0f - (1.0f / 1.0f), L"1.0");
  vSplits.emplace_back(1.0f - (3.0f / 4.0f), L".50");
  vSplits.emplace_back(1.0f - (5.0f / 8.0f), L".25");
  vSplits.emplace_back(1.0f - (9.0f / 16.0f), L"");
  vSplits.emplace_back(1.0f - (1.0f / 2.0f), L"0/1");
  vSplits.emplace_back(1.0f - (1.0f / 4.0f), L".50");
  vSplits.emplace_back(1.0f - (1.0f / 8.0f), L".25");
  vSplits.emplace_back(1.0f - (1.0f / 16.0f), L"");
  vSplits.emplace_back(1.0f, L"0.0");
}

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
  state.input = &input;
  state.output = &output;
  state.cvModel = &model.cv;
  state.spectrum = model.plot.spec;
  state.model = &model.audio.units[GroupIndex(model.plot.type, PlotType::Unit1)];
  UnitDSP::Plot(&state);
}

static void
RenderFilter(SynthModel const& model, PlotInput const& input, PlotOutput& output)
{
  FilterPlotState state;
  state.input = &input;
  state.output = &output;
  state.cvModel = &model.cv;
  state.audioModel = &model.audio;
  state.spectrum = model.plot.spec;
  state.index = GroupIndex(model.plot.type, PlotType::Filt1);
  state.model = &model.audio.filts[state.index];
  FilterDSP::Plot(&state);
}

void
PlotDSP::Render(SynthModel const& model, PlotInput const& input, PlotOutput& output)
{
  auto type = model.plot.type;
  auto index = static_cast<int>(type);
  int ampEnv = static_cast<int>(model.amp.envSrc);
  EnvModel const& envModel = model.cv.envs[ampEnv];
  int hold = model.plot.spec && (model.plot.type == PlotType::Synth || model.plot.type >= PlotType::LFO1)? SpecHold: model.plot.hold;

  switch(model.plot.type)
  {
  case PlotType::Synth: {
    SynthDSP::Plot(model, envModel, model.plot.spec, hold, input, output);
    break; }
  case PlotType::Amp: {
    AmpDSP::Plot(model.amp, envModel, model.cv, hold, input, output);
    break; }
  case PlotType::Env1: case PlotType::Env2: case PlotType::Env3: {
    auto env = static_cast<int>(PlotType::Env1);
    EnvDSP::Plot(model.cv.envs[index - env], hold, input, output);
    break; }
  case PlotType::LFO1: case PlotType::LFO2: case PlotType::LFO3: RenderLfo(model, input, output); break;
  case PlotType::Unit1: case PlotType::Unit2: case PlotType::Unit3: RenderUnit(model, input, output); break;
  case PlotType::Filt1: case PlotType::Filt2: case PlotType::Filt3: RenderFilter(model, input, output); break;
  default: assert(false); break;
  }  
  
  assert(output.rate <= input.rate);  
  if(!output.spectrum) return;  
  output.min = 0.0f;
  output.max = 1.0f;

  if (output.lSamples->empty()) return;  
  output.lSamples->resize(NextPowerOf2(output.lSamples->size()));
  Spectrum(*output.lSamples, *output.fftData, *output.fftScratch, output.rate);
  SpectrumHSplits(*output.hSplits);
  
  if (!output.stereo)
  {
    SpectrumVSplitsMono(*output.vSplits);
    return;
  }
  SpectrumVSplitsStereo(*output.vSplits);
  output.rSamples->resize(NextPowerOf2(output.rSamples->size()));
  Spectrum(*output.rSamples, *output.fftData, *output.fftScratch, output.rate);
}

} // namespace Xts