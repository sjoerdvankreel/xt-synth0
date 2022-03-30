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
  _output = x;
  if(!_model->on) return x;
  int delay = Param::SamplesI(_model->sync, _model->delay, _model->step, _bpm, _rate, XTS_DELAY_TIME_MIN_MS, XTS_DELAY_TIME_MAX_MS);
  FloatSample feedback = x + Param::Level(_model->feedback) * _line.Get(static_cast<size_t>(delay % DELAY_MAX_SAMPLES));
  _line.Push(feedback);
  float mix = Param::Level(_model->mix);
  _output = x * (1.0f - mix) + feedback * mix;
  return _output.Sanity();
}

} // namespace Xts