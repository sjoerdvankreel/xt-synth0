#include "DSP.hpp"
#include "LfoDSP.hpp"

namespace Xts {

float
LfoDSP::Generate()
{
  return 0.0f;
}

float
LfoDSP::Next()
{
	if (!_model->on) return 0.0f;
	float sample = Generate();
	_phase += Freq(*_model, *_input) / _input->rate;
	_phase -= floor(_phase);
	assert(-1.0f <= sample && sample <= 1.0f);
  return sample;
}

float
LfoDSP::Freq(LfoModel const& model, SynthInput const& input)
{
	float length = model.sync ? SyncF(input, model.step) : TimeF(model.rate, input.rate);
	return input.rate / length;
}

void
LfoDSP::Plot(LfoModel const& model, PlotInput const& input, PlotOutput& output)
{
}

} // namespace Xts