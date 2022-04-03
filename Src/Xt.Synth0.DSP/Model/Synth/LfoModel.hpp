#ifndef XTS_MODEL_SYNTH_LFO_MODEL_HPP
#define XTS_MODEL_SYNTH_LFO_MODEL_HPP

#include <Model/Shared/Model.hpp>

namespace Xts {

enum class LfoType { Sin, Saw, Sqr, Tri, Rnd };

struct XTS_ALIGN LfoModel
{
  XtsBool on;
  LfoType type;
  XtsBool sync;
  XtsBool invert;
  XtsBool unipolar;
  int32_t step;
  int32_t smooth;
  int32_t frequency;
  int32_t randomSeed;
  int32_t randomStart;
  int32_t randomNext;
  int32_t randomSlope;
};
XTS_CHECK_SIZE(LfoModel, 48);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_LFO_MODEL_HPP