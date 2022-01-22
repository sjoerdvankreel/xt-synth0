#include "DSP.hpp"
#include "EnvDSP.hpp"

#include <cmath>
#include <cassert>

namespace Xts {

struct EnvParams
{
  float dly, a, hld, d, s, r;
public:
  EnvParams() = default;
  EnvParams(EnvParams const&) = default;
};

void
EnvDSP::NextStage(EnvStage stage)
{
  _stagePos = 0;
  _stage = stage;
}

void
EnvDSP::Release()
{
  if (_stage >= EnvStage::R) return;
  bool off = _model->type == EnvType::Off;
  NextStage(off ? EnvStage::End : EnvStage::R);
}

float
EnvDSP::Generate(float from, float to, float len, int slp) const
{
  float range = to - from;
  float pos = _stagePos / len;
  float mix = Mix02Exclusive(slp);
  if (mix <= 1.0f) return from + range * powf(pos, mix);
  return from + range * (1.0f - powf(1.0f - pos, 2.0f - mix));
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

float
EnvDSP::Next()
{
  const float threshold = 1.0E-5f;
  if (_model->type == EnvType::Off || _stage == EnvStage::End) return 0.0f;
  EnvParams params = Params(*_model, *_input);
  float result = Generate(params);
  CycleStage(_model->type, params);
  assert(0.0f <= result && result <= 1.0f);
  if (_stage != EnvStage::End) _stagePos++;
  if (_stage < EnvStage::R) _level = result;
  if (_stage > EnvStage::A && result <= threshold) NextStage(EnvStage::End);
  return result;
}

EnvDSP::
EnvDSP(EnvModel const* model, AudioInput const* input) :
GeneratorDSP(model, input), _level(0.0f), _stagePos(0), _stage(EnvStage::Dly)
{
  bool off = model->type == EnvType::Off;
  NextStage(off ? EnvStage::S : EnvStage::Dly);
  CycleStage(model->type, Params(*model, *input));
}

EnvParams
EnvDSP::Params(EnvModel const& model, AudioInput const& input)
{
  EnvParams result;
  result.s = Level(model.s);
  bool sync = model.sync != 0;
  result.a = sync ? Sync(input, model.aSnc) : Time(model.a, input.rate);
  result.d = sync ? Sync(input, model.dSnc) : Time(model.d, input.rate);
  result.r = sync ? Sync(input, model.rSnc) : Time(model.r, input.rate);
  result.dly = sync ? Sync(input, model.dlySnc) : Time(model.dly, input.rate);
  result.hld = sync ? Sync(input, model.hldSnc) : Time(model.hld, input.rate);
  return result;
}

void
EnvDSP::CycleStage(EnvType type, EnvParams const& params)
{
  if (_stage == EnvStage::Dly && _stagePos >= params.dly) NextStage(EnvStage::A);
  if (_stage == EnvStage::A && _stagePos >= params.a) NextStage(EnvStage::Hld);
  if (_stage == EnvStage::Hld && _stagePos >= params.hld) NextStage(EnvStage::D);
  if (_stage == EnvStage::D && _stagePos >= params.d) NextStage(EnvStage::S);
  if (_stage == EnvStage::S && type != EnvType::DAHDSR) NextStage(EnvStage::R);
  if (_stage == EnvStage::R && _stagePos >= params.r) NextStage(EnvStage::End);
}

void
EnvDSP::Plot(EnvModel const& model, PlotInput const& input, PlotOutput& output)
{
  const float testRate = 1000.0f;
  const float sustainFactor = 0.2f;

  output.freq = 0.0f;
  output.rate = 0.0f;
  output.clip = false;
  output.bipolar = false;
  if (model.type == EnvType::Off) return;

  auto params = Params(model, AudioInput(testRate, 120, 4, UnitNote::C));
  auto length = params.dly + params.a + params.hld + params.d + params.r;
  int sustain = static_cast<int>(length * sustainFactor);
  output.rate = input.pixels / (length + sustain) * testRate;

  int i = 0;
  int s = 0;
  EnvStage prev = EnvStage::Dly;
  auto in = AudioInput(output.rate, 120, 4, UnitNote::C);
  EnvDSP dsp(&model, &in);
  while(!dsp.End())
  {
    if(s == sustain) dsp.Release();
    output.samples->push_back(dsp.Next());
    if(dsp._stage == EnvStage::S) s++;
    if(prev != dsp._stage) output.splits->push_back(s);
    prev = dsp._stage; s++; i++;
  }
}

} // namespace Xts