#ifndef XTS_MODEL_SEQUENCER_PATTERN_FX_MODEL_HPP
#define XTS_MODEL_SEQUENCER_PATTERN_FX_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <cstdint>

namespace Xts {

struct XTS_ALIGN PatternFxModel
{
  int32_t value;
  int32_t target;
};
XTS_CHECK_SIZE(PatternFxModel, 8);

} // namespace Xts
#endif // XTS_MODEL_SEQUENCER_PATTERN_FX_MODEL_HPP