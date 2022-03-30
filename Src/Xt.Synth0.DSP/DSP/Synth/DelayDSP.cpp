#include <DSP/Synth/DelayDSP.hpp>
#include <DSP/Synth/DelayPlot.hpp>
#include <Model/Synth/DelayModel.hpp>

#include <memory>

namespace Xts {

void
DelayPlot::Render(SynthModel const& model, PlotInput const& input, PlotState& state)
{ std::make_unique<DelayPlot>(&model)->DoRender(input, state); }

FloatSample 
DelayDSP::Next(FloatSample x)
{
  return x;
}

} // namespace Xts