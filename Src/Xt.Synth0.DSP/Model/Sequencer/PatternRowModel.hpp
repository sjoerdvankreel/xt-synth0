#ifndef XTS_MODEL_SEQUENCER_PATTERN_ROW_MODEL_HPP
#define XTS_MODEL_SEQUENCER_PATTERN_ROW_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Sequencer/PatternFxModel.hpp>
#include <Model/Sequencer/PatternKeyModel.hpp>
#include <Model/Sequencer/SequencerConfig.hpp>

#include <cstdint>

namespace Xts {

struct XTS_ALIGN PatternRowModel
{
  PatternFxModel fx[XTS_SEQUENCER_MAX_FXS];
  PatternKeyModel keys[XTS_SEQUENCER_MAX_KEYS];
};
XTS_CHECK_SIZE(PatternRowModel, 88);

} // namespace Xts
#endif // XTS_MODEL_SEQUENCER_PATTERN_ROW_MODEL_HPP