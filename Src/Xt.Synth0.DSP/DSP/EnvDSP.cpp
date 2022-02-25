#include "DSP.hpp"
#include "EnvDSP.hpp"
#include "PlotDSP.hpp"

#include <cmath>
#include <cassert>

namespace Xts {

static const double MaxEnv = 0.99;

EnvDSP::
EnvDSP(EnvModel const* model, float bpm, float rate) :
_pos(0), _max(0.0f), _output(),
_params(Params(*model, bpm, rate)), _model(model),
_slp(0.0), _lin(0.0), _log(0.0)
{
  _output.val = 0.0f;
  _output.staged = false;
  _output.stage = EnvStage::Dly;
  NextStage(!_model->on ? EnvStage::S : EnvStage::Dly);
  CycleStage(model->type);
}

EnvOutput
EnvDSP::Release()
{
  if (_model->on && _output.stage >= EnvStage::R) 
    return Output();
  NextStage(!_model->on ? EnvStage::End : EnvStage::R);
  CycleStage(_model->type);
  return Output();
}

EnvOutput
EnvDSP::Output() const 
{
  EnvOutput result = _output;
  result.val = _model->on && _model->inv ? 1.0f - _output.val : _output.val; 
  return result;
}

EnvOutput
EnvDSP::Next()
{
  _output.val = 0.0f;
  _output.staged = false;
  const float threshold = 1.0E-5f;
  if (!_model->on || _output.stage == EnvStage::End) return Output();
  _output.val = Generate();
  assert(0.0f <= _output.val && _output.val <= 1.0f);
  if (_output.stage != EnvStage::End) _pos++;
  if (_output.stage < EnvStage::R) _max = _output.val;
  if (_output.stage > EnvStage::A && _output.val <= threshold) NextStage(EnvStage::End);
  CycleStage(_model->type);
  return Output();
}

float
EnvDSP::Generate()
{
  switch (_output.stage)
  {
  case EnvStage::Dly: return 0.0f;
  case EnvStage::Hld: return 1.0f;
  case EnvStage::S: return _params.s;
  case EnvStage::A: return Generate(0.0, 1.0, _model->aSlp);
  case EnvStage::R: return Generate(_max, 0.0, _model->rSlp);
  case EnvStage::D: return Generate(1.0, _params.s, _model->dSlp);
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
  case SlopeType::Sin: val = from + std::sinf(slp * PI * 0.5f) * range; _slp += _lin;  break;
  case SlopeType::Cos: val = from + (1.0f - std::cosf(slp * PI * 0.5f)) * range; _slp += _lin; break;
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
  result.s = Level(model.s);
  result.a = sync ? SyncI(bpm, rate, model.aStp) : TimeI(model.a, rate);
  result.d = sync ? SyncI(bpm, rate, model.dStp) : TimeI(model.d, rate);
  result.r = sync ? SyncI(bpm, rate, model.rStp) : TimeI(model.r, rate);
  result.dly = sync ? SyncI(bpm, rate, model.dlyStp) : TimeI(model.dly, rate);
  result.hld = sync ? SyncI(bpm, rate, model.hldStp) : TimeI(model.hld, rate);
  return result;
}

void
EnvDSP::CycleStage(EnvType type)
{
  if (_output.stage == EnvStage::Dly && _pos >= _params.dly) NextStage(EnvStage::A);
  if (_output.stage == EnvStage::A && _pos >= _params.a) NextStage(EnvStage::Hld);
  if (_output.stage == EnvStage::Hld && _pos >= _params.hld) NextStage(EnvStage::D);
  if (_output.stage == EnvStage::D && _pos >= _params.d) NextStage(EnvStage::S);
  if (_output.stage == EnvStage::S && type == EnvType::DAHDR) _max = std::max(_max, _params.s);
  if (_output.stage == EnvStage::S && type == EnvType::DAHDR) NextStage(EnvStage::R);
  if (_output.stage == EnvStage::R && _pos >= _params.r) NextStage(EnvStage::End);
}

void
EnvDSP::NextStage(EnvStage stage)
{
  int len;
  SlopeType type;
  _pos = 0;
  _output.stage = stage;
  _output.staged = true;
  switch (stage)
  {
  case EnvStage::A: len = _params.a; type = _model->aSlp; break;
  case EnvStage::D: len = _params.d; type = _model->dSlp; break;
  case EnvStage::R: len = _params.r; type = _model->rSlp; break;
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
  auto end = [](EnvDSP const& dsp) { return dsp.End(); };
  auto release = [](EnvDSP& dsp) { return dsp.Release(); };
  auto envOutput = [](EnvDSP const& dsp) { return dsp.Output(); };
  auto factory = [&](float rate) { return EnvDSP(&model, input.bpm, rate); };
  auto next = [](EnvDSP& dsp, PlotOutput& output) { output.lSamples->push_back(dsp.Next().val); };
  PlotDSP::RenderStaged(hold, 0, model, input, output, factory, next, envOutput, release, end);
}

} // namespace Xts