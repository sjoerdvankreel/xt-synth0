#include "DSP.hpp"
#include "EnvDSP.hpp"
#include "UnitDSP.hpp"
#include "PlotDSP.hpp"
#include "SynthDSP.hpp"

#include <vector>
#include <complex>
#include <cassert>

namespace Xts {

static void
Spectrum(
  std::vector<float>& x, 
  std::vector<float>& specScratch,
  std::vector<std::complex<float>>& fft, 
  std::vector<std::complex<float>>& fftScratch)
{
  float max = 0;
  assert(x.size() == NextPow2(x.size()));
  Fft(x, fft, fftScratch);
  x.erase(x.begin() + x.size() / 2, x.end());
  for(size_t i = 0; i < x.size(); i++)
  {
    float real2 = fft[i].real() * fft[i].real();
    float imag2 = fft[i].imag() * fft[i].imag();
    x[i] = sqrtf(real2 + imag2);
    max = std::max(max, x[i]);
  }
  for(size_t i = 0; i < x.size(); i++) x[i] /= max;
  specScratch.clear();
  size_t count = 1;
  for(size_t i = 0; i < x.size() - 1; )
  {
    float val = 0.0f;
    for(size_t j = i; j < i + count; j++)
      val += x[j] * x[j];
    specScratch.push_back(sqrtf(val));
    i += count;
    count *= 2;
  }
  assert((1ULL << specScratch.size()) == x.size());
  x = specScratch;
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
  output.samples->resize(NextPow2(output.samples->size()));
  Spectrum(*output.samples, *output.specScratch, *output.fftData, *output.fftScratch);
}

} // namespace Xts