#include "EnvDSP.hpp"
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

float 
EnvDSP::Next(EnvModel const& env, float rate, bool active, EnvStage* stage)
{
  const float threshold = 1.0E-5f;
  if(_stage == EnvStage::End)
  {
    *stage = _stage;
    return 0.0f;
  }

  float dly, a, hld, d, r;
  float s = static_cast<float>(env.s / 255.0f);
  Length(env, rate, &dly, &a, &hld, &d, &r);  

  if(_stage == EnvStage::Dly && _stagePos >= dly)
  {
    _stagePos = 0;
    _stage = EnvStage::A;
  } 
  if (_stage == EnvStage::A && _stagePos >= a)
  {
    _stagePos = 0;
    _stage = EnvStage::Hld;
  }
  if (_stage == EnvStage::Hld && _stagePos >= hld)
  {
    _stagePos = 0;
    _stage = EnvStage::D;
  }
  if (_stage == EnvStage::D && _stagePos >= d)
  {
    _stagePos = 0;
    _stage = EnvStage::S;
  }
  if(!active && _stage != EnvStage::R)
  {
    _stagePos = 0;
    _stage = EnvStage::R;
  }
  if (_stage == EnvStage::R && _stagePos >= r)
  {
    _stagePos = 0;
    _stage = EnvStage::End;
  }

  int x = 1;
  float result = 0.0f;
  switch(_stage)
  {
  case EnvStage::S: result = s; break;
  case EnvStage::Dly: result = 0.0f; break;
  case EnvStage::Hld: result = 1.0f; break;
  case EnvStage::R: result = s + powf(_stagePos / r, env.rSlope >= 128 ? (env.rSlope - 128.0f) / x + 1 : env.rSlope / 128.0f) * (0.0f - s); break;
  case EnvStage::D: result = 1.0f + powf(_stagePos / d, env.dSlope >= 128 ? (env.dSlope - 128.0f) / x + 1 : env.dSlope / 128.0f) * (s - 1.0f); break;
  case EnvStage::A: result = 0.0f + (env.aSlope<=128.0f? powf(_stagePos / a, env.aSlope / 128.0f): 1.0f - powf(1.0f - (_stagePos / a), 1.0f-(env.aSlope-128.0f) / 128.0f)) * (1.0f - 0.0f); break;
  case EnvStage::End: result = 0.0f; break;
  default: assert(false); break;
  }

  assert(!isnan(result));
  if (_stage != EnvStage::End) _stagePos++;
  if((_stage == EnvStage::S || _stage == EnvStage::R) && result <= threshold)
  {
    _stagePos = 0;
    _stage = EnvStage::End;
  }
  *stage = _stage;
  return env.on? result: 0.0f;
}

} // namespace Xts