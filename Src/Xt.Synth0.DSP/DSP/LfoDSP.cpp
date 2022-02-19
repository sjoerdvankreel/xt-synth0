#include "DSP.hpp"
#include "LfoDSP.hpp"
#include <cmath>
#include <cassert>

namespace Xts {

static inline bool
IsBipolar(LfoPolarity plty) { return plty == LfoPolarity::Bi || plty == LfoPolarity::BiInv; }
static inline bool
IsInverted(LfoPolarity plty) { return plty == LfoPolarity::BiInv || plty == LfoPolarity::UniInv; }

LfoDSP::
LfoDSP(LfoModel const* model, float bpm, float rate):
_phase(0.0), _output(), _model(model),
_incr(Freq(*_model, bpm, rate) / rate),
_base(IsBipolar(_model->plty) ? 0.0f : 0.5f),
_factor((IsInverted(_model->plty) ? -1.0f : 1.0f)* (1.0f - _base))
{	_output.bip = IsBipolar(_model->plty); }

void
LfoDSP::Next()
{
  _output.val = 0.0f;
	if (!_model->on) return;
	_output.val = Generate();
	assert(-1.0f <= _output.val && _output.val <= 1.0f);
	_phase += _incr;
	_phase -= std::floor(_phase);
}

float
LfoDSP::Freq(LfoModel const& model, float bpm, float rate)
{
	if (model.sync) return rate / SyncF(bpm, rate, model.step);
	return rate / TimeF(model.rate, rate);
}

float
LfoDSP::Generate() const
{
	float phase = static_cast<float>(_phase);
	switch (_model->type)
	{
	case LfoType::Tri: break;
	case LfoType::Saw: return _base + _factor * (phase * 2.0f - 1.0f);
	case LfoType::Sin: return _base + _factor * sinf(phase * 2.0f * PI);
	case LfoType::Sqr: return _base + _factor * (phase < 0.5f ? 1.0f : -1.0f);
	default: assert(false); return 0.0f;
	}
	float tri = phase < 0.25f ? phase : phase < 0.75f ? 0.5f - phase : (phase - 0.75f) - 0.25f;
	return _base + _factor * tri * 4.0f;
}

void
LfoDSP::Plot(LfoModel const& model, PlotInput const& input, PlotOutput& output)
{
	const float testRate = 1000.0f;
	if (!model.on) return;
	output.max = 1.0f;
	output.freq = Freq(model, input.bpm, testRate);
	output.min = IsBipolar(model.plty) ? -1.0f : 0.0f;
	output.rate = input.spec ? input.rate : output.freq * input.pixels;

	LfoDSP dsp(&model, input.bpm, output.rate);
	float fsamples = input.spec ? output.rate : output.rate / output.freq + 1;
	int samples = static_cast<int>(std::ceilf(fsamples));
	for (int i = 0; i < samples; i++)
	{
		dsp.Next();
		output.samples->push_back(dsp.Output().val);
	}

	output.hSplits->emplace_back(0, L"0");
	output.hSplits->emplace_back(samples, L"");
	output.hSplits->emplace_back(samples / 2, L"\u03C0");
	if (IsBipolar(model.plty))
	{
		output.vSplits->emplace_back(0.0f, L"0");
		output.vSplits->emplace_back(1.0f, L"-1");
		output.vSplits->emplace_back(-1.0f, L"1");
	}
	else
	{
		output.vSplits->emplace_back(0.0f, L"1");
		output.vSplits->emplace_back(1.0f, L"0");
		output.vSplits->emplace_back(0.5f, L"\u00BD");
	}
}

} // namespace Xts