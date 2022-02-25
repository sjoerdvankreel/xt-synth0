#include "DSP.hpp"
#include "LfoDSP.hpp"
#include "PlotDSP.hpp"

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

CvOutput
LfoDSP::Next()
{
  _output.val = 0.0f;
	if (!_model->on) return Output();
	_output.val = Generate();
	assert(-1.0f <= _output.val && _output.val <= 1.0f);
	_phase += _incr;
	_phase -= std::floor(_phase);
  return Output();
}

float
LfoDSP::Freq(LfoModel const& model, float bpm, float rate)
{
	if (model.sync) return rate / SyncF(bpm, rate, model.step);
	return rate / TimeF(model.prd, rate);
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
LfoDSP::Plot(LfoModel const& model, bool spec, PlotInput const& input, PlotOutput& output)
{
	if (!model.on) return;
  PlotFlags flags = PlotNone;
  flags |= spec ? PlotSpec : 0;
  flags |= IsBipolar(model.plty)? PlotBipolar: 0;
  float freq = Freq(model, input.bpm, input.rate);
  auto next = [](LfoDSP& dsp) { return dsp.Next().val; };
  auto factory = [&](float rate) { return LfoDSP(&model, input.bpm, rate); };
  PlotDSP::RenderCycled(1, freq, flags, input, output, factory, next);
}

} // namespace Xts