#ifndef XTS_MODEL_SYNTH_LFO_MODEL_HPP
#define XTS_MODEL_SYNTH_LFO_MODEL_HPP

#include <Model/Model.hpp>

namespace Xts {

enum class LfoType { Sin, Saw, Sqr, Tri };
enum class LfoPolarity { Unipolar, UnipolarInv, Bipolar, BipolarInv };

struct XTS_ALIGN LfoModel
{
  XtsBool on;
  XtsBool sync;
  LfoType type;
  int32_t step;
  int32_t frequency;
  LfoPolarity polarity;
};
XTS_CHECK_SIZE(LfoModel, 24);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_LFO_MODEL_HPP