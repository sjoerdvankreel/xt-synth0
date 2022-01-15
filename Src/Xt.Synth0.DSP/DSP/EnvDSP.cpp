#include "EnvDSP.hpp"
#include "DSP.hpp"
#include <cmath>
#include <cassert>

namespace Xts {

void
EnvDSP::Reset()
{
  _stagePos = 0;
  _stage = EnvStage::Dly;
}

void 
EnvDSP::NextStage(EnvStage stage)
{
  _stagePos = 0;
  _stage = stage;
}

void 
EnvDSP::Length(
  EnvModel const& env, float rate, float* dly,
  float* a, float* hld, float* d, float* r) const
{
  *a = static_cast<float>(env.a * env.a * rate / 1000.0f);
  *d = static_cast<float>(env.d * env.d * rate / 1000.0f);
  *r = static_cast<float>(env.r * env.r * rate / 1000.0f);
  *dly = static_cast<float>(env.dly * env.dly * rate / 1000.0f);
  *hld = static_cast<float>(env.hld * env.hld * rate / 1000.0f);
}

void
EnvDSP::CycleStage(float dly, float a, float hld, float d, float r, bool active)
{
  if (_stage == EnvStage::Dly && _stagePos >= dly) NextStage(EnvStage::A);
  if (_stage == EnvStage::A && _stagePos >= a) NextStage(EnvStage::Hld);
  if (_stage == EnvStage::Hld && _stagePos >= hld) NextStage(EnvStage::D);
  if (_stage == EnvStage::D && _stagePos >= d) NextStage(EnvStage::S);
  if (!active && _stage != EnvStage::R) NextStage(EnvStage::R);
  if (_stage == EnvStage::R && _stagePos >= r) NextStage(EnvStage::End);
}

float
EnvDSP::Generate(EnvModel const& env, float a, float d, float r) const
{
  float s = Level(env.s);
  switch (_stage)
  {
  case EnvStage::S: return s; 
  case EnvStage::Dly: return 0.0f; 
  case EnvStage::Hld: return 1.0f; 
  case EnvStage::End: return 0.0f; 
  case EnvStage::R: return Generate(s, 0.0, _stagePos / r, env.rSlope); break;
  case EnvStage::D: return Generate(1.0, s, _stagePos / d, env.dSlope); break;
  case EnvStage::A: return Generate(0.0, 1.0, _stagePos / a, env.aSlope); break;
  default: assert(false); return 0.0f;
  }
}

float
EnvDSP::Generate(float from, float to, float pos, int slope) const
{
  float range = to - from;
  float slopef = static_cast<float>(slope);
  if(slopef <= 128.0f) return from + range * powf(pos, slopef / 128.0f);
  return from + range * (1.0f - powf(1.0f - pos, 1.0f - (slopef - 128.0f) / 128.0f));
}

float 
EnvDSP::Next(EnvModel const& env, float rate, bool active, bool plot, EnvStage* stage)
{
  if(!plot && !env.on)
  {
    *stage = EnvStage::End;
    return 1.0f;
  }

  if(_stage == EnvStage::End)
  {
    *stage = _stage;
    return 0.0f;
  }

  float dly, a, hld, d, r;
  const float threshold = 1.0E-5f;
  Length(env, rate, &dly, &a, &hld, &d, &r);  
  CycleStage(dly, a, hld, d, r, active);
  float result = Generate(env, a, d, r);
  assert(!isnan(result));
  if(_stage != EnvStage::End) _stagePos++;
  if((_stage == EnvStage::S || _stage == EnvStage::R) && result <= threshold) NextStage(EnvStage::End);
  *stage = _stage;
  return result;
}

} // namespace Xts