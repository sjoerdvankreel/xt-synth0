#ifndef XTS_MODEL_SYNTH_LFO_MODEL_HPP
#define XTS_MODEL_SYNTH_LFO_MODEL_HPP

#include <Model/Shared/Model.hpp>

namespace Xts {

enum class LfoShape { Bi, Uni, BiInv, UniInv };
enum class LfoType { Sin, Saw, Sqr, Tri, Rnd1, Rnd2, Rnd3, Rnd4, Rnd5, Rnd6, Rnd7, Rnd8 , Rnd9, Rnd10 };

struct XTS_ALIGN LfoModel
{
  XtsBool on;
  LfoType type;
  XtsBool sync;
  LfoShape shape;
  int32_t step;
  int32_t smooth;
  int32_t frequency;
  int32_t randomSeed;
  int32_t randomSpeed;
  int32_t randomSteepness;
};
XTS_CHECK_SIZE(LfoModel, 40);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_LFO_MODEL_HPP