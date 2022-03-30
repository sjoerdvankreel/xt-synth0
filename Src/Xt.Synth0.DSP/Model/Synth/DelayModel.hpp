#ifndef XTS_MODEL_SYNTH_DELAY_MODEL_HPP
#define XTS_MODEL_SYNTH_DELAY_MODEL_HPP

#include <Model/Shared/Model.hpp>

namespace Xts {

struct XTS_ALIGN DelayModel
{
  XtsBool on;
  XtsBool sync;
  int32_t mix;
  int32_t step;
  int32_t delay;
  int32_t feedback;
};
XTS_CHECK_SIZE(DelayModel, 24);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_DELAY_MODEL_HPP