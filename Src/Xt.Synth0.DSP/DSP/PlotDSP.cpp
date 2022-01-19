#include "EnvDSP.hpp"
#include "UnitDSP.hpp"
#include "PlotDSP.hpp"
#include "SynthDSP.hpp"
#include <cassert>

namespace Xts {

void
PlotDSP::Render(SynthModel const& model, PlotInput const& input, PlotOutput& output)
{
  output.splits->clear();
  output.samples->clear();
  auto type = model.plot.type;
  auto index = static_cast<int>(type);
  switch(model.plot.type)
  {
  case PlotType::Synth: {
    SynthDSP().Plot(model, input, output); 
    break; }
  case PlotType::Env1: case PlotType::Env2: {
    auto env = static_cast<int>(PlotType::Env1);
    EnvDSP().Plot(model.envs[index - env], input, output); 
    break; }
  case PlotType::Unit1: case PlotType::Unit2: case PlotType::Unit3: {
    auto unit = static_cast<int>(PlotType::Unit1);
    UnitDSP().Plot(model.units[index - unit], input, output); 
    break; }
  default: {
    assert(false);
    break; }
  }
}

} // namespace Xts