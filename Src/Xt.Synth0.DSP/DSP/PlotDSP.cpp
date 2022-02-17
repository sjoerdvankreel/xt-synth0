#include "DSP.hpp"
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
    if(i == 1) marker = L"\u00BD";
    if(i == 2) marker = L"\u00BC";
    if(i == 3) marker = L"\u215B";
    vSplits.emplace_back(VSplit(split, marker));
  }
  vSplits.emplace_back(VSplit(1.0f, L"0"));
}

void
PlotDSP::Render(SynthModel const& synth, PlotInput& input, PlotOutput& output)
{
  auto type = synth.plot.type;
  input.hold = synth.plot.hold;
  input.spec = synth.plot.spec;
  auto index = static_cast<int>(type);
  output.channel = type == PlotType::SynthR? 1: 0;

  switch(synth.plot.type)
  {
  case PlotType::Off: break;
  case PlotType::SynthL: case PlotType::SynthR: {
    SynthDSP().Plot(synth, synth.source, input, output);
    break; }
  case PlotType::Amp: {
    AmpDSP().Plot(synth.amp, synth.source, input, output);
    break; }
  case PlotType::LFO1: case PlotType::LFO2: {
    auto lfo = static_cast<int>(PlotType::LFO1);
    LfoDSP().Plot(synth.source.lfos[index - lfo], input, output);
    break; }
  case PlotType::Env1: case PlotType::Env2: case PlotType::Env3: {
    auto env = static_cast<int>(PlotType::Env1);
    EnvDSP().Plot(synth.source.envs[index - env], input, output);
    break; }
  case PlotType::Unit1: case PlotType::Unit2: case PlotType::Unit3: {
    auto unit = static_cast<int>(PlotType::Unit1);
    UnitDSP().Plot(synth.units[index - unit], synth.source, input, output);
    break; }
  default: {
    assert(false);
    break; }
  }
  
  if(!synth.plot.spec) return;
  output.min = 0.0f;
  output.max = 1.0f;
  output.samples->resize(NextPow2(output.samples->size()));
  if(output.samples->empty()) return;
  Spectrum(*output.samples, *output.hSplits, *output.vSplits, *output.fftData, *output.fftScratch, output.rate);
}

} // namespace Xts