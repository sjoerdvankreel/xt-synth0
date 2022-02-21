#include "DSP.hpp"
#include "AmpDSP.hpp"
#include "EnvDSP.hpp"
#include "UnitDSP.hpp"
#include "PlotDSP.hpp"
#include "SynthDSP.hpp"
#include "../Utility/Utility.hpp"

#include <vector>
#include <complex>
#include <cassert>

namespace Xts {

// https://stackoverflow.com/questions/604453/analyze-audio-using-fast-fourier-transform
// https://dsp.stackexchange.com/questions/46692/calculating-1-3-octave-spectrum-from-fft-dft
static float
Power(std::vector<std::complex<float>>& fft, float rate, int oct, int note)
{
  float result = 0.0f;
  float freq2Bin = rate / (fft.size() * 2.0f);
  float midi = static_cast<float>(oct * 12 + note);
  size_t bin1 = static_cast<size_t>(Freq(midi) * freq2Bin);
  size_t bin2 = static_cast<size_t>(Freq(midi + 1) * freq2Bin);
  for (size_t i = bin1; i < bin2 && i < fft.size(); i++)
    result += fft[i].real() * fft[i].real() + fft[i].imag() * fft[i].imag();
  return sqrtf(result);
}

static void
Spectrum(
  std::vector<float>& x, 
  std::vector<HSplit>& hSplits,
  std::vector<VSplit>& vSplits,
  std::vector<std::complex<float>>& fft, 
  std::vector<std::complex<float>>& fftScratch,
  float rate)
{
  int i = 0;
  float max = 0;
  assert(x.size() > 0 && x.size() == NextPow2(x.size()));
  Fft(x, fft, fftScratch);

  x.clear();
  hSplits.clear();
  fft.erase(fft.begin() + fft.size() / 2, fft.end());  
  for(int oct = 0; oct < 12; oct++)
  {
    std::wstring marker = oct >= 2? std::to_wstring(oct - 2): L"";
    hSplits.emplace_back(HSplit(i, marker));
    for(int note = 0; note < 12; note++, i++)
      x.push_back(Power(fft, rate, oct, note));
  }    
  hSplits.emplace_back(HSplit(143, L""));
  for (size_t i = 0; i < x.size(); i++) max = std::max(x[i], max);
  for (size_t i = 0; i < x.size(); i++) x[i] /= max;

  vSplits.clear();
  for(int i = 0; i < 7; i++)
  {
    float split = 1.0f - 1.0f / (1 << i);
    std::wstring marker = L"";
    if(i == 0) marker = L"1";
    if(i == 1) marker = std::wstring(1, UnicodeOneHalf);
    if(i == 2) marker = std::wstring(1, UnicodeOneQuarter);
    if(i == 3) marker = std::wstring(1, UnicodeOneEight);
    vSplits.emplace_back(VSplit(split, marker));
  }
  vSplits.emplace_back(VSplit(1.0f, L"0"));
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
    AmpDSP::Plot(model.amp, envModel, model.cv, model.audio, hold, input, output);
    break; }
  case PlotType::Env1: case PlotType::Env2: case PlotType::Env3: {
    auto env = static_cast<int>(PlotType::Env1);
    EnvDSP::Plot(model.cv.envs[index - env], hold, input, output);
    break; }
  case PlotType::LFO1: case PlotType::LFO2: case PlotType::LFO3: {
    auto lfo = static_cast<int>(PlotType::LFO1);
    LfoDSP::Plot(model.cv.lfos[index - lfo], model.plot.spec, input, output);
    break; }
  case PlotType::Unit1: case PlotType::Unit2: case PlotType::Unit3: {
    auto unit = static_cast<int>(PlotType::Unit1);
    UnitDSP::Plot(model.audio.units[index - unit], model.cv, model.plot.spec, input, output);
    break; }
  default: {
    assert(false);
    break; }
  }  

  assert(output.rate <= input.rate);
  if(!output.spec) return;  

  output.min = 0.0f;
  output.max = 1.0f;
  if (output.lSamples->empty()) return;
  output.lSamples->resize(NextPow2(output.lSamples->size()));
  Spectrum(*output.lSamples, *output.hSplits, *output.vSplits, *output.fftData, *output.fftScratch, output.rate);
  if(!output.stereo) return;

  output.hSplits->clear();
  output.vSplits->clear();
  output.rSamples->resize(NextPow2(output.rSamples->size()));
  Spectrum(*output.rSamples, *output.hSplits, *output.vSplits, *output.fftData, *output.fftScratch, output.rate);
}

} // namespace Xts