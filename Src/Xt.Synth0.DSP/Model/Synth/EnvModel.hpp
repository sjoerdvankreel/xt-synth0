#ifndef XTS_MODEL_SYNTH_ENV_MODEL_HPP
#define XTS_MODEL_SYNTH_ENV_MODEL_HPP

#include <Model/Model.hpp>

namespace Xts {

enum class EnvType { DAHDSR, DAHDR };
enum class SlopeType { Lin, Log, Inv, Sin, Cos };

struct XTS_ALIGN EnvModel
{
  EnvType type;
  XtsBool on;
  XtsBool sync;
  XtsBool invert;
  
  int32_t delayTime;
  int32_t attackTime;
  int32_t holdTime;
  int32_t decayTime;
  int32_t releaseTime;

  int32_t delayStep;
  int32_t attackStep;
  int32_t holdStep;
  int32_t decayStep;
  int32_t releaseStep;

  int32_t sustain;
  SlopeType decaySlope;
  SlopeType attackSlope;
  SlopeType releaseSlope;
};
XTS_CHECK_SIZE(EnvModel, 72);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_ENV_MODEL_HPP