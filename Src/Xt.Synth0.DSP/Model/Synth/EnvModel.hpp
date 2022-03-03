#ifndef XTS_MODEL_SYNTH_ENV_MODEL_HPP
#define XTS_MODEL_SYNTH_ENV_MODEL_HPP

#include <Model/Model.hpp>

namespace Xts {

enum class EnvType { DAHDSR, DAHDR };
enum class SlopeType { Lin, Log, Inv, Sin, Cos };
struct XTS_ALIGN EnvModel
{
  EnvType type;
  XtsBool on, sync, inv;
  SlopeType aSlp, dSlp, rSlp;
  int32_t dly, a, hld, d, s, r;
  int32_t dlyStp, aStp, hldStp, dStp, rStp;
};
XTS_CHECK_SIZE(EnvModel, 72);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_ENV_MODEL_HPP