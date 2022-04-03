#include <DSP/Shared/Param.hpp>
#include <DSP/Synth/LfoDSP.hpp>
#include <Model/Synth/SynthModel.hpp>

#include <cmath>
#include <memory>
#include <cassert>

#define MIN_FREQ_HZ 0.1f
#define MAX_FREQ_HZ 20.0f

namespace Xts {

static bool
LfoIsBipolar(LfoShape shape)
{ return shape == LfoShape::Bi || shape == LfoShape::BiInv; }
static bool
LfoIsInverted(LfoShape shape)
{ return shape == LfoShape::UniInv || shape == LfoShape::BiInv; }

PeriodicParams
LfoPlot::Params() const
{
  PeriodicParams result;
  result.periods = 1;
  result.autoRange = false;
  result.allowResample = true;
  result.bipolar = LfoIsBipolar(_model->shape);
  return result;
}

void
LfoPlot::Render(SynthModel const& model, PlotInput const& input, PlotState& state)
{
  LfoModel const* lfo = &model.global.lfo;
  if(model.global.plot.type != PlotType::GlobalLFO)
  {
    int base = static_cast<int>(PlotType::LFO1);
    int type = static_cast<int>(model.global.plot.type);
    lfo = &model.voice.cv.lfos[type - base];
  }
  if (lfo->on) LfoPlot(lfo).DoRender(input, state);
}

LfoDSP::
LfoDSP(LfoModel const* model, float bpm, float rate) :
LfoDSP()
{
  _bpm = bpm;
  _rate = rate;
  _phase = 0.0;
  _model = model;
  _prng = Prng(static_cast<uint32_t>(model->randomSeed));
}

CvSample
LfoDSP::Next()
{
  _output.value = 0.0f;
  _output.bipolar = LfoIsBipolar(_model->shape);
  if (!_model->on) return Output();
  _output.value = Generate();
  _phase += Frequency(*_model, _bpm, _rate) / _rate;
  if (_phase >= 1.0) _prng = Prng(static_cast<uint32_t>(_model->randomSeed));
  _phase -= std::floor(_phase);
  return Output().Sanity();
}

float
LfoDSP::Frequency(LfoModel const& model, float bpm, float rate)
{
  if (model.sync) return rate / Param::StepSamplesF(model.step, bpm, rate);
  return Param::Frequency(static_cast<float>(model.frequency), MIN_FREQ_HZ, MAX_FREQ_HZ);
}

float
LfoDSP::Generate()
{
  switch (_model->type)
  {
  case LfoType::Rnd1: case LfoType::Rnd2: case LfoType::Rnd3: return GenerateRandom();
  case LfoType::Sin: case LfoType::Saw: case LfoType::Sqr: case LfoType::Tri: return GenerateWave();
  default: assert(false); return 0.0f;
  }
}

float
LfoDSP::GenerateWave() const
{
  float phase = static_cast<float>(_phase);
  float base = LfoIsBipolar(_model->shape)? 0.0f : 0.5f;
  float factor = (LfoIsInverted(_model->shape) ? -1.0f : 1.0f) * (1.0f - base);
	switch (_model->type)
	{
	case LfoType::Tri: break;
	case LfoType::Saw: return base + factor * (phase * 2.0f - 1.0f);
  case LfoType::Sqr: return base + factor * (phase < 0.5f ? 1.0f : -1.0f);
  case LfoType::Sin: return base + factor * std::sinf(phase * 2.0f * PIF);
	default: assert(false); return 0.0f;
	}
	float tri = phase < 0.25f ? phase : phase < 0.75f ? 0.5f - phase : (phase - 0.75f) - 0.25f;
	return base + factor * tri * 4.0f;
}

float
LfoDSP::GenerateRandom()
{
  return 0.0f;
}

} // namespace Xts