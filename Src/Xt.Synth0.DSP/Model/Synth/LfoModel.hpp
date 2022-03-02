#ifndef XTS_MODEL_SYNTH_LFO_MODEL_HPP
#define XTS_MODEL_SYNTH_LFO_MODEL_HPP

#include <Model/Model.hpp>

namespace Xts {

enum class LfoType { Sin, Saw, Sqr, Tri };
enum class LfoPolarity { Uni, UniInv, Bi, BiInv };

struct XTS_ALIGN LfoModel
{
  LfoType type;
  LfoPolarity plty;
  XtsBool on, sync;
  int32_t frq, step;
};
XTS_CHECK_SIZE(LfoModel, 24);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_LFO_MODEL_HPP