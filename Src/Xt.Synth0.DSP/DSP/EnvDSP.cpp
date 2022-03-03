#include "DSP.hpp"
#include "EnvDSP.hpp"
#include "PlotDSP.hpp"
#include <DSP/Param.hpp>
#include <DSP/Utility.hpp>

#include <cmath>
#include <cassert>

#define MIN_TIME_MS 0.0f
#define MAX_TIME_MS 10000.0f

namespace Xts {

static const double MaxEnv = 0.99;

EnvDSP::
EnvDSP(EnvModel const* model, float bpm, float rate) :
_pos(0), _max(0.0f), _output(),
_params(Params(*model, bpm, rate)), _model(model),
_slp(0.0), _lin(0.0), _log(0.0)
{
  _output.value = 0.0f;
  _output.switchedStage = false;
  _output.stage = EnvStage::Delay;
  NextStage(!_model->on ? EnvStage::Sustain : EnvStage::Delay);
  CycleStage(model->type);
}

EnvSample
EnvDSP::Release()
{
  if (_model->on && _output.stage >= EnvStage::Release)
    return Output();
  NextStage(!_model->on ? EnvStage::End : EnvStage::Release);
  CycleStage(_model->type);
  return Output();
}

EnvSample
EnvDSP::Output() const 
{
  EnvSample result = _output;
  result.value = _model->on && _model->inv ? 1.0f - _output.value : _output.value; 
  return result;
}

EnvSample
EnvDSP::Next()
{
  _output.value = 0.0f;
  _output.switchedStage = false;
  const float threshold = 1.0E-5f;
  if (!_model->on || _output.stage == EnvStage::End) return Output();
  _output.value = Generate();
  assert(0.0f <= _output.value && _output.value <= 1.0f);
  if (_output.stage != EnvStage::End) _pos++;
  if (_output.stage < EnvStage::Release) _max = _output.value;
  if (_output.stage > EnvStage::Attack && _output.value <= threshold) NextStage(EnvStage::End);
  CycleStage(_model->type);
  return Output();
}

float
EnvDSP::Generate()
{
  switch (_output.stage)
  {
  case EnvStage::Delay: return 0.0f;
  case EnvStage::Hold: return 1.0f;
  case EnvStage::Sustain: return _params.s;
  case EnvStage::Attack: return Generate(0.0, 1.0, _model->aSlp);
  case EnvStage::Release: return Generate(_max, 0.0, _model->rSlp);
  case EnvStage::Decay: return Generate(1.0, _params.s, _model->dSlp);
  default: assert(false); return 0.0f;
  }
}

float
EnvDSP::Generate(float from, float to, SlopeType type)
{
  float val = 0.0f;
  float range = to - from;
  float slp = static_cast<float>(_slp);
  switch (type)
  {
  case SlopeType::Lin: val = from + slp * range; _slp += _lin; break;
  case SlopeType::Log: val = from + (slp - 1.0f) * range; _slp *= _log; break;
  case SlopeType::Inv: val = from + (2.0f - slp * 2.0f) * range; _slp /= _log; break;
  case SlopeType::Sin: val = from + std::sinf(slp * PIF * 0.5f) * range; _slp += _lin;  break;
  case SlopeType::Cos: val = from + (1.0f - std::cosf(slp * PIF * 0.5f)) * range; _slp += _lin; break;
  default: assert(false); break;
  }
  assert(to < from || from <= val && val <= to);
  assert(to >= from || to <= val && val <= from);
  assert(type == SlopeType::Log || 0.0 <= _slp && _slp <= 1.0);
  assert(type != SlopeType::Log || 1.0 <= _slp && _slp <= 2.0);
  return val;
}

EnvParams
EnvDSP::Params(EnvModel const& model, float bpm, float rate)
{
  EnvParams result;
  bool sync = model.sync != 0;
  result.s = Param::Level(model.s);
  result.a = sync ? Param::StepFramesI(model.aStp, bpm, rate) : Param::TimeFramesI(model.a, rate, MIN_TIME_MS, MAX_TIME_MS);
  result.d = sync ? Param::StepFramesI(model.dStp, bpm, rate) : Param::TimeFramesI(model.d, rate, MIN_TIME_MS, MAX_TIME_MS);
  result.r = sync ? Param::StepFramesI(model.rStp, bpm, rate) : Param::TimeFramesI(model.r, rate, MIN_TIME_MS, MAX_TIME_MS);
  result.dly = sync ? Param::StepFramesI(model.dlyStp, bpm, rate) : Param::TimeFramesI(model.dly, rate, MIN_TIME_MS, MAX_TIME_MS);
  result.hld = sync ? Param::StepFramesI(model.hldStp, bpm, rate) : Param::TimeFramesI(model.hld, rate, MIN_TIME_MS, MAX_TIME_MS);
  return result;
}

void
EnvDSP::CycleStage(EnvType type)
{
  if (_output.stage == EnvStage::Delay && _pos >= _params.dly) NextStage(EnvStage::Attack);
  if (_output.stage == EnvStage::Attack && _pos >= _params.a) NextStage(EnvStage::Hold);
  if (_output.stage == EnvStage::Hold && _pos >= _params.hld) NextStage(EnvStage::Decay);
  if (_output.stage == EnvStage::Decay && _pos >= _params.d) NextStage(EnvStage::Sustain);
  if (_output.stage == EnvStage::Sustain && type == EnvType::DAHDR) _max = std::max(_max, _params.s);
  if (_output.stage == EnvStage::Sustain && type == EnvType::DAHDR) NextStage(EnvStage::Release);
  if (_output.stage == EnvStage::Release && _pos >= _params.r) NextStage(EnvStage::End);
}

void
EnvDSP::NextStage(EnvStage stage)
{
  int len;
  SlopeType type;
  _pos = 0;
  _output.stage = stage;
  _output.switchedStage = true;
  switch (stage)
  {
  case EnvStage::Attack: len = _params.a; type = _model->aSlp; break;
  case EnvStage::Decay: len = _params.d; type = _model->dSlp; break;
  case EnvStage::Release: len = _params.r; type = _model->rSlp; break;
  default: len = -1; type = SlopeType::Lin;  break;
  }
  if (len == -1) return;
  switch (type)
  {
  case SlopeType::Lin: case SlopeType::Sin: case SlopeType::Cos: _slp = 0.0, _lin = MaxEnv / len; break;
  case SlopeType::Log: case SlopeType::Inv: _slp = 1.0, _log = std::pow(1.0 + MaxEnv, 1.0 / len); break;
  default: assert(false); break;
  }
}

void
EnvDSP::Plot(EnvModel const& model, int hold, PlotInput const& input, PlotOutput& output)
{
  if(!model.on) return;
  auto next = [](EnvDSP& dsp) { dsp.Next(); };
  auto end = [](EnvDSP const& dsp) { return dsp.End(); };
  auto release = [](EnvDSP& dsp) { return dsp.Release(); };
  auto val = [](EnvDSP const& dsp) { return dsp.Output().value; };
  auto envOutput = [](EnvDSP const& dsp) { return dsp.Output(); };
  auto factory = [&](float rate) { return EnvDSP(&model, input.bpm, rate); };
  PlotDSP::RenderStaged(hold, 0, model, input, output, factory, next, val, val, envOutput, release, end);
}

} // namespace Xts