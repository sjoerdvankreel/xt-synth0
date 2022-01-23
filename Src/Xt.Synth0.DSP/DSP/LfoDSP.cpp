#include "DSP.hpp"
#include "LfoDSP.hpp"
#include <cmath>
#include <cassert>

namespace Xts {

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
LfoDSP::Generate()
{
  float phase = static_cast<float>(_phase);
  switch(_model->type)
  {
    case LfoType::Saw: return BasicSaw(phase);
		case LfoType::Sin: return BasicSin(phase);
		case LfoType::Sqr: return BasicSqr(phase);
		case LfoType::Tri: return BasicTri(phase);
		default: assert(false); return 0.0f;
	}
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
  float testRate = 1000.0f;
	SynthInput testIn(testRate, input.bpm, 4, UnitNote::C);
	output.freq = 0.0f;
	output.rate = 0.0f;
	output.clip = false;
	output.bipolar = true;

	if (!model.on) return;
	output.freq = Freq(model, testIn);
	output.rate = output.freq * input.pixels;

	SynthInput in(output.rate, input.bpm, 4, UnitNote::C);
	LfoDSP dsp(&model, &in);
	float samples = output.rate / output.freq;
	for (int i = 0; i < static_cast<int>(samples); i++)
		output.samples->push_back(dsp.Next());
}

} // namespace Xts