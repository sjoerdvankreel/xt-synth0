#include "DSP.hpp"
#include "EnvDSP.hpp"
#include "UnitDSP.hpp"
#include "PlotDSP.hpp"
#include "SynthDSP.hpp"

#include <vector>
#include <complex>
#include <cassert>

namespace Xts {

static uint64_t
NextPow2(uint64_t x)
{
  uint64_t result = 0;
  if (x && !(x & (x - 1))) return x;
  while (x != 0) x >>= 1, result++;
  return 1ULL << result;
}

static void
Fft(
  std::complex<float>* x, 
  std::complex<float>* scratch, 
  size_t count)
{
  assert(count == NextPow2(count));
  if(count < 2) return;  
  std::complex<float>* even = scratch;
  std::complex<float>* odd = scratch + count / 2;
  for(size_t i = 0; i < count / 2; i++) even[i] = x[i * 2];
  for(size_t i = 0; i < count / 2; i++) odd[i] = x[i * 2 + 1];
  Fft(odd, x, count / 2);
  Fft(even, x, count / 2);
  for (size_t i = 0; i < count / 2; i++)
  {
    float im = -2.0f * PI * i / count;
    std::complex<float> t = std::polar(1.0f, im) * odd[i];
    x[i] = even[i] + t;
    x[i + count/2] = even[i] - t;
  }
}

static void
Fft(
  std::vector<float>& x, 
  std::vector<std::complex<float>>& fft, 
  std::vector<std::complex<float>>& scratch)
{
  assert(x.size() == NextPow2(x.size()));
  fft.resize(x.size());
  scratch.resize(x.size());
  for(size_t i = 0; i < x.size(); i++)
    fft[i] = std::complex<float>(x[i], 0.0f);
  Fft(fft.data(), scratch.data(), x.size());
}

static void
Spectrum(
  std::vector<float>& x, 
  std::vector<std::complex<float>>& fft, 
  std::vector<std::complex<float>>& scratch)
{
  float max = 0;
  assert(x.size() == NextPow2(x.size()));
  Fft(x, fft, scratch);
  x.erase(x.begin() + x.size() / 2, x.end());
  for(size_t i = 0; i < x.size(); i++)
  {
    float real2 = fft[i].real() * fft[i].real();
    float imag2 = fft[i].imag() * fft[i].imag();
    x[i] = sqrtf(real2 + imag2);
    max = std::max(max, x[i]);
  }
  for(size_t i = 0; i < x.size(); i++) x[i] /= max;
}

// TODO fft not dft
void
Dft(std::vector<float>& yn, std::vector<std::complex<float>>& scratch)
{
  int N = yn.size();
  std::vector<float> xn = yn;
  std::vector<float> Xr;
  std::vector<float> Xi;
  Xr.resize(xn.size());
  Xi.resize(xn.size());

  for(int i = 0; i < N; i++)xn[i]=xn[i]*2-1;

  int len = N;
  for (int k = 0; k < N; k++) {
    Xr[k] = 0;
    Xi[k] = 0;
    for (int n = 0; n < len; n++) {
      Xr[k]
        = (Xr[k]
          + xn[n] * cos(2 * 3.141592 * k * n / N));
      Xi[k]
        = (Xi[k]
          + xn[n] * sin(2 * 3.141592 * k * n / N));
    }
  }
  float max = 0;
  for(int i = 0; i < N; i++) 
  {
    xn[i]=sqrt(Xr[i]* Xr[i] + Xi[i]*Xi[i]);
    max = xn[i]>max?xn[i]:max;
  }
  for (int i = 0; i < N; i++)xn[i]/=max;
  yn.clear();
  for(int i = 0; i < N/2;i++)yn.push_back(xn[i]);
  
  int i=0;
  i++;
}

void
PlotDSP::Render(SynthModel const& synth, PlotInput& input, PlotOutput& output)
{
  auto type = synth.plot.type;
  auto index = static_cast<int>(type);
  input.hold = synth.plot.hold;
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
//if(synth.plot.spec) Dft(*output.samples, *output.fft);
  output.samples->resize(NextPow2(output.samples->size()));
  Spectrum(*output.samples, *output.fftData, *output.fftScratch);
}

} // namespace Xts