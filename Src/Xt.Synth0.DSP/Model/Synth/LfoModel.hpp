#ifndef XTS_MODEL_SYNTH_LFO_MODEL_HPP
#define XTS_MODEL_SYNTH_LFO_MODEL_HPP

#include <Model/Shared/Model.hpp>

namespace Xts {

enum class LfoType { Sin, Saw, Sqr, Tri };

struct XTS_ALIGN LfoModel
{
  XtsBool on;
  LfoType type;
  XtsBool sync;
  XtsBool invert;
  XtsBool unipolar;
  int32_t step;
  int32_t frequency;
  int32_t pad__;
};
XTS_CHECK_SIZE(LfoModel, 32);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_LFO_MODEL_HPP