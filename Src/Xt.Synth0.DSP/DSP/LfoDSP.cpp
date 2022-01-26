#include "DSP.hpp"
#include "LfoDSP.hpp"
#include <cmath>
#include <cassert>

namespace Xts {

float
LfoDSP::Next()
{
	if (!_model->on) return 1.0f;
	float sample = Generate();
	_phase += Freq(*_model, *_input) / _input->rate;
	_phase -= floor(_phase);
	assert(-1.0f <= sample && sample <= 1.0f);
  return sample;
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

	SynthInput testIn(testRate, input.bpm, 4, UnitNote::C);
	output.bipolar = model.bi;
	output.freq = Freq(model, testIn);
	output.rate = output.freq * input.pixels;

	SynthInput in(output.rate, input.bpm, 4, UnitNote::C);
	LfoDSP dsp(&model, &in);
	float samples = output.rate / output.freq;
	for (int i = 0; i < static_cast<int>(samples); i++)
		output.samples->push_back(dsp.Next());
}

float
LfoDSP::Freq(LfoModel const& model, SynthInput const& input)
{
	float length = model.sync ? SyncF(input, model.step) : TimeF(model.rate, input.rate);
	return input.rate / length;
}

} // namespace Xts