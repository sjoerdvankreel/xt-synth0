#ifndef XTS_MODEL_SEQUENCER_PATTERN_ROW_MODEL_HPP
#define XTS_MODEL_SEQUENCER_PATTERN_ROW_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Shared/SharedConfig.hpp>
#include <Model/Sequencer/PatternFxModel.hpp>
#include <Model/Sequencer/PatternKeyModel.hpp>

#include <cstdint>

namespace Xts {

struct XTS_ALIGN PatternRowModel
{
  PatternFxModel fx[XTS_SHARED_MAX_FXS];
  PatternKeyModel keys[XTS_SHARED_MAX_KEYS];
};
XTS_CHECK_SIZE(PatternRowModel, 88);

} // namespace Xts
#endif // XTS_MODEL_SEQUENCER_PATTERN_ROW_MODEL_HPP