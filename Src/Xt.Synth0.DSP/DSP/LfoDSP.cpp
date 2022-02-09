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
LfoDSP::Freq(LfoModel const& model, SourceInput const& input)
{
	float length = model.sync ? SyncF(input, model.step) : TimeF(model.rate, input.rate);
	return input.rate / length;
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
    case LfoType::Saw: return base + factor * (1.0f - phase * 2.0f);
		case LfoType::Sin: return base + factor * sinf(phase * 2.0f * PI);
		case LfoType::Sqr: return base + factor * (phase < 0.5f ? 1.0f : -1.0f);
		case LfoType::Tri: break;
		default: assert(false); return 0.0f;
	}
	float tri = (phase < 0.25f ? phase : phase < 0.75f ? 0.5f - phase : -0.25f + (phase - 0.75f)) * 4.0f;
	return base + factor * tri;
}

void
LfoDSP::Plot(LfoModel const& model, PlotInput const& input, PlotOutput& output)
{
	const float testRate = 1000.0f;
	if (!model.on) return;
	SourceInput testIn(testRate, input.bpm);
	output.max = 1.0f;
  output.min = model.bi? -1.0f: 0.0f;
	output.freq = Freq(model, testIn);
	output.rate = input.spec ? input.rate : output.freq * input.pixels;

	SourceInput in(output.rate, input.bpm);
	LfoDSP dsp(&model, &in);
  float fsamples = input.spec ? output.rate : output.rate / output.freq + 1;
	int samples = static_cast<int>(std::ceilf(fsamples));
	for (int i = 0; i < samples; i++)
  {
    dsp.Next();
		output.samples->push_back(dsp.Value());
  }

	output.hSplits->emplace_back(HSplit(0, L"0"));
	output.hSplits->emplace_back(HSplit(samples, L""));
	output.hSplits->emplace_back(HSplit(samples / 2, L"\u03C0"));
	if (model.bi)
	{
		output.vSplits->emplace_back(VSplit(0.0f, L"0"));
		output.vSplits->emplace_back(VSplit(1.0f, L"-1"));
		output.vSplits->emplace_back(VSplit(-1.0f, L"1"));
	}
	else
	{
		output.vSplits->emplace_back(VSplit(0.0f, L"1"));
		output.vSplits->emplace_back(VSplit(1.0f, L"0"));
		output.vSplits->emplace_back(VSplit(0.5f, L"\u00BD"));
	}
}

} // namespace Xts