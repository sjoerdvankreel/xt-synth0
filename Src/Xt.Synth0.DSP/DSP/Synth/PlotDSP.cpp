#include <DSP/Synth/AmpDSP.hpp>
#include <DSP/Synth/EnvDSP.hpp>
#include <DSP/Synth/LfoDSP.hpp>
#include <DSP/Synth/PlotDSP.hpp>
#include <DSP/Synth/UnitDSP.hpp>
#include <DSP/Synth/VoiceDSP.hpp>
#include <DSP/Synth/FilterPlot.hpp>
#include <DSP/Shared/Utility.hpp>

namespace Xts {

void
SynthPlotRender(SynthModel const& model, PlotInput const& input, PlotState& state)
{
  state.hold = model.plot.hold;
  switch(model.plot.type)
  {
  case PlotType::Amp: AmpPlot::Render(model, input, state); break;
  case PlotType::Synth: VoicePlot::Render(model, input, state); break;
  case PlotType::Env1: case PlotType::Env2: case PlotType::Env3: EnvPlot::Render(model, input, state); break;
  case PlotType::LFO1: case PlotType::LFO2: case PlotType::LFO3: LfoPlot::Render(model, input, state); break;
  case PlotType::Unit1: case PlotType::Unit2: case PlotType::Unit3: UnitPlot::Render(model, input, state); break;
  case PlotType::Filter1: case PlotType::Filter2: case PlotType::Filter3: FilterPlot::Render(model, input, state); break;
  default: assert(false); break;
  }    
}

} // namespace Xts