#include "DSP.hpp"
#include "EnvDSP.hpp"
#include "UnitDSP.hpp"
#include "PlotDSP.hpp"
#include "SynthDSP.hpp"

#include <vector>
#include <complex>
#include <cassert>

namespace Xts {

static float
Power(std::vector<std::complex<float>>& fft, int oct, int note)
{
  float result = 0.0f;
  float midi = oct * 12 + note;
  size_t freq1 = static_cast<size_t>(Freq(midi));
  size_t freq2 = static_cast<size_t>(Freq(midi + 1));
  for (size_t i = freq1; i < freq2 && i < fft.size(); i++)
    result += fft[i].real() * fft[i].real() + fft[i].imag() * fft[i].imag();
  return sqrtf(result);
}

static void
Spectrum(
  std::vector<float>& x, 
  std::vector<float>& specScratch0,
  std::vector<std::complex<float>>& fft, 
  std::vector<std::complex<float>>& fftScratch)
{
  float max = 0;
  assert(x.size() == NextPow2(x.size()));
  Fft(x, fft, fftScratch);
  x.clear();
  fft.erase(fft.begin() + fft.size() / 2, fft.end());  
  for(int oct = 0; oct < 12; oct++)
    for(int note = 0; note < 12; note++)
      x.push_back(Power(fft, oct, note));
  for(size_t i = 0; i < x.size(); i++) max = std::max(x[i], max);
  for(size_t i = 0; i < x.size(); i++) x[i] /= max;
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
  case PlotType::Global: {
    GlobalDSP().Plot(synth.global, synth.source, input, output);
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
  output.bipolar = false;
  output.splits->clear();
  output.samples->resize(NextPow2(output.samples->size()));
  Spectrum(*output.samples, *output.specScratch, *output.fftData, *output.fftScratch);
}

} // namespace Xts