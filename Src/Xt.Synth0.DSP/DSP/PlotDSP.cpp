#include "EnvDSP.hpp"
#include "UnitDSP.hpp"
#include "PlotDSP.hpp"
#include "SynthDSP.hpp"
#include <cassert>

namespace Xts {

void
PlotDSP::Render(SynthModel const& synth, PlotInput const& input, PlotOutput& output)
{
  auto type = synth.plot.type;
  auto index = static_cast<int>(type);

  output.splits->clear();
  output.samples->clear();
  output.channel = type == PlotType::SynthR? 1: 0;

  switch(synth.plot.type)
  {
  case PlotType::SynthL: case PlotType::SynthR: {
    SynthDSP().Plot(synth, input, output);
    break; }
  case PlotType::Env1: case PlotType::Env2: case PlotType::Env3: {
    auto env = static_cast<int>(PlotType::Env1);
    EnvDSP().Plot(synth.envs[index - env], input, output);
    break; }
  case PlotType::Unit1: case PlotType::Unit2: case PlotType::Unit3: {
    auto unit = static_cast<int>(PlotType::Unit1);
    UnitDSP().Plot(synth.units[index - unit], input, output);
    break; }
  default: {
    assert(false);
    break; }
  }
}

} // namespace Xts