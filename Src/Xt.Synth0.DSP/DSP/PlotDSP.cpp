#include "DSP.hpp"
#include "AmpDSP.hpp"
#include "EnvDSP.hpp"
#include "UnitDSP.hpp"
#include "PlotDSP.hpp"
#include "SynthDSP.hpp"

#include <vector>
#include <complex>
#include <cassert>

namespace Xts {

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
  vSplits.emplace_back(1.0f - (1.0f / 1.0f), L"1");
  vSplits.emplace_back(1.0f - (1.0f / 2.0f), std::wstring(1, UnicodeOneHalf));
  vSplits.emplace_back(1.0f - (1.0f / 4.0f), std::wstring(1, UnicodeOneQuarter));
  vSplits.emplace_back(1.0f - (1.0f / 8.0f), std::wstring(1, UnicodeOneEight));
  vSplits.emplace_back(1.0f - (1.0f / 16.0f), L"");
  vSplits.emplace_back(1.0f - (1.0f / 32.0f), L"");
  vSplits.emplace_back(1.0f, L"0");
}

static void
SpectrumVSplitsStereo(std::vector<VSplit>& vSplits)
{
  vSplits.clear();
  vSplits.emplace_back(1.0f - (1.0f / 1.0f), L"1");
  vSplits.emplace_back(1.0f - (3.0f / 4.0f), std::wstring(1, UnicodeOneHalf));
  vSplits.emplace_back(1.0f - (5.0f / 8.0f), std::wstring(1, UnicodeOneQuarter));
  vSplits.emplace_back(1.0f - (9.0f / 16.0f), L"");
  vSplits.emplace_back(1.0f - (1.0f / 2.0f), L"01");
  vSplits.emplace_back(1.0f - (1.0f / 4.0f), std::wstring(1, UnicodeOneHalf));
  vSplits.emplace_back(1.0f - (1.0f / 8.0f), std::wstring(1, UnicodeOneQuarter));
  vSplits.emplace_back(1.0f - (1.0f / 16.0f), L"");
  vSplits.emplace_back(1.0f, L"0");
}

// https://stackoverflow.com/questions/604453/analyze-audio-using-fast-fourier-transform
// https://dsp.stackexchange.com/questions/46692/calculating-1-3-octave-spectrum-from-fft-dft
static float
Power(std::vector<std::complex<float>>& fft, float rate, int oct, int note)
{
  float result = 0.0f;
  float freq2Bin = rate / (fft.size() * 2.0f);
  float midi = static_cast<float>(oct * 12 + note);
  size_t bin1 = static_cast<size_t>(FreqNote(midi) * freq2Bin);
  size_t bin2 = static_cast<size_t>(FreqNote(midi + 1) * freq2Bin);
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
  assert(x.size() > 0 && x.size() == NextPow2(x.size()));
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
  case PlotType::LFO1: case PlotType::LFO2: case PlotType::LFO3: {
    auto lfo = static_cast<int>(PlotType::LFO1);
    LfoDSP::Plot(model.cv.lfos[index - lfo], model.plot.spec, input, output);
    break; }
  case PlotType::Unit1: case PlotType::Unit2: case PlotType::Unit3: {
    auto unit = static_cast<int>(PlotType::Unit1);
    UnitDSP::Plot(model.audio.units[index - unit], model.cv, model.plot.spec, input, output);
    break; }
  case PlotType::Filt1: case PlotType::Filt2: case PlotType::Filt3: {
    auto filt = static_cast<int>(PlotType::Filt1);
    FilterDSP::Plot(model.audio.filts[index - filt], model.cv, model.audio, model.plot.spec, index - filt, input, output);
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
  Spectrum(*output.lSamples, *output.fftData, *output.fftScratch, output.rate);
  SpectrumHSplits(*output.hSplits);
  
  if (!output.stereo)
  {
    SpectrumVSplitsMono(*output.vSplits);
    return;
  }
  SpectrumVSplitsStereo(*output.vSplits);
  output.rSamples->resize(NextPow2(output.rSamples->size()));
  Spectrum(*output.rSamples, *output.fftData, *output.fftScratch, output.rate);
}

} // namespace Xts