#include "DSP.hpp"
#include "LfoDSP.hpp"
#include <cmath>
#include <cassert>

namespace Xts {

void
LfoDSP::Next()
{
  _value = 0.0f;
	if (!_model->on) return;
	_value = Generate();
	assert(-1.0f <= _value && _value <= 1.0f);
	_phase += Freq(*_model, *_input) / _input->rate;
	_phase -= floor(_phase);
}

float
LfoDSP::Generate()
{
  float base = _model->bi? 0.0f: 0.5f;
	float inv = _model->inv ? -1.0f : 1.0f;
	float phase = static_cast<float>(_phase);
	float factor = inv * (_model->bi ? 1.0f: 0.5f);
  switch(_model->type)
  {
    case LfoType::Saw: return base + factor * BasicSaw(phase);
		case LfoType::Sin: return base + factor * BasicSin(phase);
		case LfoType::Sqr: return base + factor * BasicSqr(phase);
		case LfoType::Tri: return base + factor * BasicTri(phase);
		default: assert(false); return 0.0f;
	}
}

void
LfoDSP::Plot(LfoModel const& model, PlotInput const& input, PlotOutput& output)
{
	const float testRate = 1000.0f;
	if (!model.on) return;

	SourceInput testIn(testRate, input.bpm);
	output.bipolar = model.bi;
	output.freq = Freq(model, testIn);
	output.rate = output.freq * input.pixels;

	SourceInput in(output.rate, input.bpm);
	LfoDSP dsp(&model, &in);
	float samples = output.rate / output.freq;
	for (int i = 0; i < static_cast<int>(samples); i++)
  {
    dsp.Next();
		output.samples->push_back(dsp.Value());
  }
}

float
LfoDSP::Freq(LfoModel const& model, SourceInput const& input)
{
	float length = model.sync ? SyncF(input, model.step) : TimeF(model.rate, input.rate);
	return input.rate / length;
}

} // namespace Xts