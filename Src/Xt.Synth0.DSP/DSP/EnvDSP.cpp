#include "EnvDSP.hpp"
#include "DSP.hpp"
#include <cmath>
#include <cassert>
#include <cstring>
#include <cassert>

namespace Xts {

float 
EnvDSP::Frames(EnvParams const& params)
{ return params.dly + params.a + params.hld + params.d + params.r; }

void
EnvDSP::NextStage(EnvStage stage)
{
  _stagePos = 0;
  _stage = stage;
}

void
EnvDSP::Release(EnvModel const& env)
{
  auto type = static_cast<EnvType>(env.type);
  if (type == EnvType::Off) NextStage(EnvStage::End);
  else if (_stage < EnvStage::R) NextStage(EnvStage::R);
}

void
EnvDSP::Init(EnvModel const& env)
{
  _level = 0.0f;
  auto type = static_cast<EnvType>(env.type);
  NextStage(type == EnvType::Off? EnvStage::S: EnvStage::Dly);
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
EnvDSP::Generate(EnvModel const& env, EnvParams const& params) const
{
  auto type = static_cast<EnvType>(env.type);
  if(type == EnvType::Off) return 1.0f;
  switch (_stage)
  {
  case EnvStage::Dly: return 0.0f; 
  case EnvStage::Hld: return 1.0f; 
  case EnvStage::End: return 0.0f; 
  case EnvStage::S: return params.s;
  case EnvStage::A: return Generate(0.0, 1.0, params.a, env.aSlp);
  case EnvStage::R: return Generate(_level, 0.0, params.r, env.rSlp);
  case EnvStage::D: return Generate(1.0, params.s, params.d, env.dSlp);
  default: assert(false); return 0.0f;
  }
}

EnvParams
EnvDSP::Params(EnvModel const& env, float rate, int bpm) const
{
  EnvParams result = {};
  bool sync = env.sync != 0;
  auto type = static_cast<EnvType>(env.type);
  if(type == EnvType::Off) return result.s = 1.0f, result;
  result.s = type == EnvType::DAHDR? 0.0f: Level(env.s);
  result.a = sync ? Sync(env.aSnc, rate, bpm) : Time(env.a, rate);
  result.d = sync ? Sync(env.dSnc, rate, bpm) : Time(env.d, rate);
  result.r = sync ? Sync(env.rSnc, rate, bpm) : Time(env.r, rate);
  result.dly = sync ? Sync(env.dlySnc, rate, bpm) : Time(env.dly, rate);
  result.hld = sync ? Sync(env.hldSnc, rate, bpm) : Time(env.hld, rate);
  return result;
}

void
EnvDSP::CycleStage(EnvModel const& env, EnvParams const& params)
{
  auto type = static_cast<EnvType>(env.type);
  if(type == EnvType::Off) return;
  if (_stage == EnvStage::Dly && _stagePos >= params.dly) NextStage(EnvStage::A);
  if (_stage == EnvStage::A && _stagePos >= params.a) NextStage(EnvStage::Hld);
  if (_stage == EnvStage::Hld && _stagePos >= params.hld) NextStage(EnvStage::D);
  if (_stage == EnvStage::D && _stagePos >= params.d) NextStage(EnvStage::S);
  if (_stage == EnvStage::S && type != EnvType::DAHDSR) NextStage(EnvStage::R);
  if (_stage == EnvStage::R && _stagePos >= params.r) NextStage(EnvStage::End);
}

void 
EnvDSP::Next(EnvModel const& env, float rate, int bpm, EnvOutput& output)
{
  const float threshold = 1.0E-5f;
  memset(&output, 0, sizeof(output));
  output.stage = _stage;
  if(_stage == EnvStage::End) return;
  EnvParams params = Params(env, rate, bpm);
  CycleStage(env, params);
  float result = Generate(env, params);
  if(_stage != EnvStage::End) _stagePos++;
  if(_stage > EnvStage::A && _stage != EnvStage::End && result <= threshold) NextStage(EnvStage::End);
  if(_stage < EnvStage::R) _level = result;
  output.lvl = result;
  output.stage = _stage;
  output.staged = _stage != _prevStage;
  _prevStage = _stage;
  assert(0.0f <= output.lvl && output.lvl <= 1.0f);
}

} // namespace Xts