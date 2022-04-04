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
  result.periods = 2;
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
  InitRandom();
}

CvSample
LfoDSP::Next()
{
  _output.value = 0.0f;
  _output.bipolar = LfoIsBipolar(_model->shape);
  if (!_model->on) return Output();
  float frequency = Frequency(*_model, _bpm, _rate) / _rate;
  _output.value = Generate(1.0f / frequency);
  _phase += frequency;
  if (_phase >= 1.0) InitRandom();
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
LfoDSP::Generate(float period)
{
  float base = LfoIsBipolar(_model->shape) ? 0.0f : 0.5f;
  float factor = (LfoIsInverted(_model->shape) ? -1.0f : 1.0f) * (1.0f - base);
  switch (_model->type)
  {
  case LfoType::Sin: case LfoType::Saw: case LfoType::Sqr: case LfoType::Tri: return base + factor * GenerateWave();
  case LfoType::Rnd1: case LfoType::Rnd2: case LfoType::Rnd3: case LfoType::Rnd4: return base + factor * GenerateRandom(period);
  default: assert(false); return 0.0f;
  }
}

float
LfoDSP::GenerateWave() const
{
  float phase = static_cast<float>(_phase);
	switch (_model->type)
	{
	case LfoType::Tri: break;
	case LfoType::Saw: return phase * 2.0f - 1.0f;
  case LfoType::Sqr: return phase < 0.5f ? 1.0f : -1.0f;
  case LfoType::Sin: return std::sinf(phase * 2.0f * PIF);
	default: assert(false); return 0.0f;
	}
	float tri = phase < 0.25f ? phase : phase < 0.75f ? 0.5f - phase : (phase - 0.75f) - 0.25f;
	return tri * 4.0f;
}

void
LfoDSP::InitRandom()
{
  _randCount = 0;
  _randDir = 1.0f;
  _randLevel = 0.0f;
  _prng = Prng(std::numeric_limits<uint32_t>::max() / (_model->randomSeed + 1));
  _randState = _prng.Next() * 2.0f - 1.0f;
}

float
LfoDSP::GenerateRandom(float period)
{
  float steepness = Param::Level(_model->randomSteepness);
  if (_randCount == 0)
  {
    _randCount = NextRandomCount(period);
    _randLevel = NextRandomLevel(steepness);
    _randState = NextRandomState(steepness);
  }
  _randState += _randLevel * steepness * _randDir;
  if (_randState > 1.0f) _randState = 1.0f - (_randState - 1.0f), _randDir *= -1.0f;
  if (_randState < -1.0f) _randState = -1.0f - (_randState + 1.0f), _randDir *= -1.0f;
  _randCount--;
  return BipolarSanity(_randState);
}

int
LfoDSP::NextRandomCount(float period)
{
  int count = static_cast<int>((1.0f - Param::Level(_model->randomSpeed)) * period);
  switch (_model->type)
  {
  case LfoType::Rnd1: case LfoType::Rnd3: return count;
  case LfoType::Rnd2: case LfoType::Rnd4: return static_cast<int>(_prng.Next() * count);
  default: assert(false); return 0;
  }
}

float
LfoDSP::NextRandomLevel(float steepness)
{
  switch (_model->type)
  {
  case LfoType::Rnd1: case LfoType::Rnd2: return 0.0f;
  case LfoType::Rnd3: case LfoType::Rnd4: return (_prng.Next() * 2.0f - 1.0f) * steepness * _randDir;
  default: assert(false); return 0.0f;
  }
}

float
LfoDSP::NextRandomState(float steepness)
{
  switch (_model->type)
  {
  case LfoType::Rnd1: case LfoType::Rnd2: return _randState + steepness * (_prng.Next() * 2.0f - 1.0f);
  case LfoType::Rnd3: case LfoType::Rnd4: return _randState;
  default: assert(false); return 0.0f;
  }
}

} // namespace Xts