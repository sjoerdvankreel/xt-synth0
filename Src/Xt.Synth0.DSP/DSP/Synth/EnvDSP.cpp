#include <DSP/Shared/Param.hpp>
#include <DSP/Synth/EnvDSP.hpp>
#include <Model/Synth/SynthModel.hpp>

#include <cmath>
#include <cassert>
#include <algorithm>

#define ENV_MAX_VALUE 0.99
#define ENV_THRESHOLD 1e-5f

#define ENV_MIN_TIME_MS 0.0f
#define ENV_MAX_TIME_MS 10000.0f

namespace Xts {

StagedParams
EnvPlot::Params() const
{
  StagedParams result;
  result.stereo = false;
  result.bipolar = false;
  result.allowResample = true;
  result.allowSpectrum = false;
  return result;
}

float
EnvPlot::ReleaseSamples(EnvModel const& model, float bpm, float rate)
{
  XtsBool sync = model.sync;
  int32_t time = model.releaseTime;
  int32_t step = model.releaseStep;
  return Param::SamplesF(sync, time, step, bpm, rate, ENV_MIN_TIME_MS, ENV_MAX_TIME_MS);
}

void 
EnvPlot::Render(SynthModel const& model, PlotInput const& input, PlotState& state)
{
  int base = static_cast<int>(PlotType::Env1);
  int type = static_cast<int>(model.plot.type);
  EnvModel const* env = &model.voice.cv.envs[type - base];
  if (env->on) EnvPlot(env).DoRender(input, state);
}

static inline float
GenerateStage(float from, float base, float range, SlopeType slope)
{
  switch (slope)
  {
  case SlopeType::Lin: return from + base * range;
  case SlopeType::Log: return from + (base - 1.0f) * range;
  case SlopeType::Inv: return from + (2.0f - base * 2.0f) * range;
  case SlopeType::Sin: return from + std::sinf(base * PIF * 0.5f) * range;
  case SlopeType::Cos: return from + (1.0f - std::cosf(base * PIF * 0.5f)) * range;
  default: assert(false); return 0.0f;
  }
}

static inline double
Increment(double base, double increment, SlopeType type)
{
  switch (type)
  {
  case SlopeType::Log: return base * increment;
  case SlopeType::Inv: return base / increment;
  case SlopeType::Lin: case SlopeType::Sin: case SlopeType::Cos: return base + increment;
  default: assert(false); return 0.0;
  }
}

static bool
InitStage(EnvModel const& model, EnvParams const& params, EnvStage stage, int& length, SlopeType& type)
{
  switch (stage)
  {
  case EnvStage::Decay: type = model.decaySlope; length = params.decaySamples; return true;
  case EnvStage::Attack: type = model.attackSlope; length = params.attackSamples; return true;
  case EnvStage::Release: type = model.releaseSlope; length = params.releaseSamples; return true;
  default: return false;
  }
}

static void
InitSlope(SlopeType type, int length, double& base, double& increment)
{
  switch (type)
  {
  case SlopeType::Log: case SlopeType::Inv: base = 1.0, increment = std::pow(1.0 + ENV_MAX_VALUE, 1.0 / length); break;
  case SlopeType::Lin: case SlopeType::Sin: case SlopeType::Cos: base = 0.0, increment = ENV_MAX_VALUE / length; break;
  default: assert(false); break;
  }
}

EnvDSP::
EnvDSP(EnvModel const* model, float bpm, float rate):
EnvDSP()
{
  _pos = 0;
  _max = 0.0f;
  _base = 0.0;
  _model = model;
  _increment = 0.0;
  _output.value = 0.0f;
  _output.switchedStage = false;
  _output.stage = EnvStage::Delay;
  _params = Params(*model, bpm, rate);
  NextStage(!_model->on ? EnvStage::Sustain : EnvStage::Delay);
  CycleStage(model->type);
}

float
EnvDSP::Generate(float from, float to, SlopeType type)
{
  float range = to - from;
  float base = static_cast<float>(_base);
  _base = Increment(_base, _increment, type);
  return GenerateStage(from, base, range, type);
}

void
EnvDSP::NextStage(EnvStage stage)
{
  int length;
  SlopeType type;

  _pos = 0;
  _output.stage = stage;
  _output.switchedStage = true;
  if (!InitStage(*_model, _params, stage, length, type)) return;
  InitSlope(type, length, _base, _increment);
}

EnvSample
EnvDSP::Output() const
{
  EnvSample result = _output;
  if (_model->on && _model->invert) result.value = 1.0f - result.value;
  return result;
}

EnvSample
EnvDSP::Release()
{
  if (_model->on && _output.stage >= EnvStage::Release) return Output();
  NextStage(!_model->on ? EnvStage::End : EnvStage::Release);
  CycleStage(_model->type);
  return Output();
}

float
EnvDSP::Generate()
{
  switch (_output.stage)
  {
  case EnvStage::Hold: return 1.0f;
  case EnvStage::Delay: return 0.0f;
  case EnvStage::Sustain: return _params.sustain;
  case EnvStage::Attack: return Generate(0.0, 1.0, _model->attackSlope);
  case EnvStage::Release: return Generate(_max, 0.0, _model->releaseSlope);
  case EnvStage::Decay: return Generate(1.0, _params.sustain, _model->decaySlope);
  default: assert(false); return 0.0f;
  }
}

EnvSample
EnvDSP::Next()
{
  _output.value = 0.0f;
  _output.switchedStage = false;
  if (!_model->on || _output.stage == EnvStage::End) return Output();
  _output.value = Generate();
  if (_output.stage != EnvStage::End) _pos++;
  if (_output.stage < EnvStage::Release) _max = _output.value;
  if (_output.stage > EnvStage::Attack && _output.value <= ENV_THRESHOLD) NextStage(EnvStage::End);
  CycleStage(_model->type);
  return Output().Sanity();
}

void
EnvDSP::CycleStage(EnvType type)
{
  if (_output.stage == EnvStage::Delay && _pos >= _params.delaySamples) NextStage(EnvStage::Attack);
  if (_output.stage == EnvStage::Attack && _pos >= _params.attackSamples) NextStage(EnvStage::Hold);
  if (_output.stage == EnvStage::Hold && _pos >= _params.holdSamples) NextStage(EnvStage::Decay);
  if (_output.stage == EnvStage::Decay && _pos >= _params.decaySamples) NextStage(EnvStage::Sustain);
  if (_output.stage == EnvStage::Sustain && type == EnvType::DAHDR) _max = std::max(_max, _params.sustain);
  if (_output.stage == EnvStage::Sustain && type == EnvType::DAHDR) NextStage(EnvStage::Release);
  if (_output.stage == EnvStage::Release && _pos >= _params.releaseSamples) NextStage(EnvStage::End);
}

EnvParams
EnvDSP::Params(EnvModel const& model, float bpm, float rate)
{
  EnvParams result;
  result.sustain = Param::Level(model.sustain);
  result.holdSamples = Param::SamplesI(model.sync, model.holdTime, model.holdStep, bpm, rate, ENV_MIN_TIME_MS, ENV_MAX_TIME_MS);
  result.delaySamples = Param::SamplesI(model.sync, model.delayTime, model.delayStep, bpm, rate, ENV_MIN_TIME_MS, ENV_MAX_TIME_MS);
  result.decaySamples = Param::SamplesI(model.sync, model.decayTime, model.decayStep, bpm, rate, ENV_MIN_TIME_MS, ENV_MAX_TIME_MS);
  result.attackSamples = Param::SamplesI(model.sync, model.attackTime, model.attackStep, bpm, rate, ENV_MIN_TIME_MS, ENV_MAX_TIME_MS);
  result.releaseSamples = Param::SamplesI(model.sync, model.releaseTime, model.releaseStep, bpm, rate, ENV_MIN_TIME_MS, ENV_MAX_TIME_MS);
  return result;
}

} // namespace Xts