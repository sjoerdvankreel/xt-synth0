#include "DSP.hpp"
#include "EnvDSP.hpp"
#include "UnitDSP.hpp"
#include "PlotDSP.hpp"
#include "SynthDSP.hpp"

#include <vector>
#include <complex>
#include <cassert>

namespace Xts {

// TODO fft not dft
void
Fft(std::vector<float>& yn, std::vector<std::complex<float>>& scratch)
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
  output.channel = type == PlotType::SynthR? 1: 0;
  input.hold = synth.plot.hold;

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
  if(synth.plot.spec) Fft(*output.samples, *output.fft);
}

} // namespace Xts