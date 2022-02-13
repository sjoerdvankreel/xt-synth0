#include "DSP.hpp"
#include "EnvDSP.hpp"
#include "SourceDSP.hpp"

#include <cmath>
#include <cassert>

namespace Xts {

static const double MaxEnv = 0.99;

EnvDSP::
EnvDSP(EnvModel const* model, SourceInput const* input) :
DSPBase(model, input), 
_pos(0), _max(0.0f), _stage(EnvStage::Dly),
_params(Params(*model, *input)),
_slp(0.0), _lin(0.0), _log(0.0)
{
  NextStage(!_model->on ? EnvStage::S : EnvStage::Dly);
  CycleStage(model->type);
}

void
EnvDSP::Release()
{
  if (_model->on && _stage >= EnvStage::R) return;
  NextStage(!_model->on ? EnvStage::End : EnvStage::R);
  CycleStage(_model->type);
}

void
EnvDSP::NextStage(EnvStage stage)
{
  int len;
  _pos = 0;
  _slp = 0.0;
  _lin = 0.0;
  _log = 0.0;
  _stage = stage;
  switch (stage)
  {
  case EnvStage::A: len = _params.a; break;
  case EnvStage::D: len = _params.d; break;
  case EnvStage::R: len = _params.r; break;
  default: len = -1; break;
  }
  if (len == -1) return;
  _lin = MaxEnv / len;
  _log = std::pow(1.0 + MaxEnv, 1.0 / static_cast<double>(len));
}

void
EnvDSP::Next()
{
  _value = 0.0f;
  const float threshold = 1.0E-5f;
  if (!_model->on || _stage == EnvStage::End) return;
  _value = Generate();
  assert(0.0f <= _value && _value <= 1.0f);
  if (_stage != EnvStage::End) _pos++;
  if (_stage < EnvStage::R) _max = _value;
  if (_stage > EnvStage::A && _value <= threshold) NextStage(EnvStage::End);
  CycleStage(_model->type);
}

float
EnvDSP::Generate(float from, float to, int len, SlopeType type)
{
  double slp = type == SlopeType::Inv ? (1.0 - _slp) : _slp;
  float val = from + static_cast<float>(slp) * (to - from);
  assert(0.0f <= val && val <= 1.0f);
  if (type == SlopeType::Lin) _slp += _lin;
  else _slp *= _log;
  assert(0.0 <= _slp && _slp <= 1.0);
  return val;
}

float
EnvDSP::Generate()
{
  switch (_stage)
  {
  case EnvStage::Dly: return 0.0f;
  case EnvStage::Hld: return 1.0f;
  case EnvStage::S: return _params.s;
  case EnvStage::A: return Generate(0.0, 1.0, _params.a, _model->aSlp);
  case EnvStage::R: return Generate(_max, 0.0, _params.r, _model->rSlp);
  case EnvStage::D: return Generate(1.0, _params.s, _params.d, _model->dSlp);
  default: assert(false); return 0.0f;
  }
}

EnvParams
EnvDSP::Params(EnvModel const& model, SourceInput const& input)
{
  EnvParams result;
  bool sync = model.sync != 0;
  result.s = Level(model.s);
  result.a = sync ? SyncI(input, model.aStp) : TimeI(model.a, input.rate);
  result.d = sync ? SyncI(input, model.dStp) : TimeI(model.d, input.rate);
  result.r = sync ? SyncI(input, model.rStp) : TimeI(model.r, input.rate);
  result.dly = sync ? SyncI(input, model.dlyStp) : TimeI(model.dly, input.rate);
  result.hld = sync ? SyncI(input, model.hldStp) : TimeI(model.hld, input.rate);
  return result;
}

void
EnvDSP::CycleStage(EnvType type)
{
  if (_stage == EnvStage::Dly && _pos >= _params.dly) NextStage(EnvStage::A);
  if (_stage == EnvStage::A && _pos >= _params.a) NextStage(EnvStage::Hld);
  if (_stage == EnvStage::Hld && _pos >= _params.hld) NextStage(EnvStage::D);
  if (_stage == EnvStage::D && _pos >= _params.d) NextStage(EnvStage::S);
  if (_stage == EnvStage::S && type == EnvType::DAHDR) _max = std::max(_max, _params.s);
  if (_stage == EnvStage::S && type == EnvType::DAHDR) NextStage(EnvStage::R);
  if (_stage == EnvStage::R && _pos >= _params.r) NextStage(EnvStage::End);
}

void
EnvDSP::Plot(EnvModel const& model, PlotInput const& input, PlotOutput& output)
{
  int i = 0;
  int h = 0;
  bool firstMarker = true;
  auto prev = EnvStage::Dly;
  const float testRate = input.spec? input.rate: 1000.0f;

  if (!model.on) return;
  bool dahdsr = model.type == EnvType::DAHDSR;
  auto params = Params(model, SourceInput(testRate, input.bpm));
  int hold = TimeI(input.hold, testRate);
  int fixed = params.dly + params.a + params.hld + params.d;
  int release = dahdsr ? hold : std::min(hold, fixed);
  
  output.min = 0.0;
  output.max = 1.0;
  output.rate = input.spec? input.rate: input.pixels * testRate / (release + params.r);
  hold = static_cast<int>(hold * output.rate / testRate);
  auto in = SourceInput(output.rate, input.bpm);
  EnvDSP dsp(&model, &in);
  while(true)
  {
    if(h++ == hold) dsp.Release();
    if(dsp.End()) break;
    dsp.Next();
    output.samples->push_back(dsp.Value());
    if((firstMarker || prev != dsp._stage) && !dsp.End())
    {
      firstMarker = false;
      std::wstring marker = L"";
      if(dsp._stage == EnvStage::A) marker = L"A";
      if(dsp._stage == EnvStage::D) marker = L"D";
      if(dsp._stage == EnvStage::S) marker = L"S";
      if(dsp._stage == EnvStage::R) marker = L"R";
      if(dsp._stage == EnvStage::Dly) marker = L"D";
      if(dsp._stage == EnvStage::Hld) marker = L"H";
      output.hSplits->push_back(HSplit(i, marker));
    }
    prev = dsp._stage; 
    i++;
  }
  output.hSplits->push_back(HSplit(i - 1, L""));
  output.vSplits->emplace_back(VSplit(0.0f, L"1"));
  output.vSplits->emplace_back(VSplit(1.0f, L"0"));
  output.vSplits->emplace_back(VSplit(0.5f, L"\u00BD"));
  output.vSplits->emplace_back(VSplit(0.25f, L"\u00BE"));
  output.vSplits->emplace_back(VSplit(0.75f, L"\u00BC"));
}

} // namespace Xts