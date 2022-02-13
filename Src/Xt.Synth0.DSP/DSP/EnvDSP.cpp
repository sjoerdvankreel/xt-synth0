#include "DSP.hpp"
#include "EnvDSP.hpp"
#include "SourceDSP.hpp"

#include <cmath>
#include <cassert>

namespace Xts {

struct EnvParams
{
  float s;
  int dly, a, hld, d, r;
public:
  EnvParams() = default;
  EnvParams(EnvParams const&) = default;
};

void
EnvDSP::NextStage(EnvStage stage)
{
  _pos = 0;
  _stage = stage;
}

void
EnvDSP::Release()
{
  if (_model->on && _stage >= EnvStage::R) return;
  NextStage(!_model->on ? EnvStage::End : EnvStage::R);
  CycleStage(_model->type, Params(*_model, *_input));
}

EnvDSP::
EnvDSP(EnvModel const* model, SourceInput const* input) :
DSPBase(model, input), _pos(0), _level(0.0f), _stage(EnvStage::Dly)
{
  NextStage(!_model->on ? EnvStage::S : EnvStage::Dly);
  CycleStage(model->type, Params(*model, *input));
}

float
EnvDSP::Generate(float from, float to, int len, int slp) const
{
  float range = to - from;
  float pos = _pos / static_cast<float>(len);
  assert(0.0f <= pos && pos <= 1.0f);
  float mix = MixUni2(slp);
  if (mix <= 1.0f) 
  { 
    float slope = powf(pos, mix);
    assert(0.0f <= slope && slope <= 1.0f);
    return from + range * slope;
  }
  float slope = powf(1.0f - pos, 2.0f - mix);
  assert(0.0f <= slope && slope <= 1.0f);
  return from + range * (1.0f - slope);
}

float
EnvDSP::Generate(EnvParams const& params) const
{
  switch (_stage)
  {
  case EnvStage::Dly: return 0.0f; 
  case EnvStage::Hld: return 1.0f; 
  case EnvStage::S: return params.s;
  case EnvStage::A: return Generate(0.0, 1.0, params.a, _model->aSlp);
  case EnvStage::R: return Generate(_level, 0.0, params.r, _model->rSlp);
  case EnvStage::D: return Generate(1.0, params.s, params.d, _model->dSlp);
  default: assert(false); return 0.0f;
  }
}

void
EnvDSP::Next()
{
  _value = 0.0f;
  const float threshold = 1.0E-5f;
  if (!_model->on || _stage == EnvStage::End) return;
  EnvParams params = Params(*_model, *_input);
  _value = Generate(params);
  assert(0.0f <= _value && _value <= 1.0f);
  if (_stage != EnvStage::End) _pos++;
  if (_stage < EnvStage::R) _level = _value;
  if (_stage > EnvStage::A && _value <= threshold) NextStage(EnvStage::End);
  CycleStage(_model->type, params);
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
EnvDSP::CycleStage(EnvType type, EnvParams const& params)
{
  if (_stage == EnvStage::Dly && _pos >= params.dly) NextStage(EnvStage::A);
  if (_stage == EnvStage::A && _pos >= params.a) NextStage(EnvStage::Hld);
  if (_stage == EnvStage::Hld && _pos >= params.hld) NextStage(EnvStage::D);
  if (_stage == EnvStage::D && _pos >= params.d) NextStage(EnvStage::S);
  if (_stage == EnvStage::S && type == EnvType::DAHDR) _level = std::max(_level, params.s);
  if (_stage == EnvStage::S && type == EnvType::DAHDR) NextStage(EnvStage::R);
  if (_stage == EnvStage::R && _pos >= params.r) NextStage(EnvStage::End);
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