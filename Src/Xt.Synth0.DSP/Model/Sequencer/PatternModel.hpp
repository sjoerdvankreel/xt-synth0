#ifndef XTS_MODEL_SEQUENCER_PATTERN_MODEL_HPP
#define XTS_MODEL_SEQUENCER_PATTERN_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Sequencer/PatternRowModel.hpp>
#include <Model/Sequencer/SequencerConfig.hpp>

#include <cstdint>

namespace Xts {

struct XTS_ALIGN PatternModel
{
  PatternRowModel rows[XTS_SEQUENCER_TOTAL_ROWS];
};
XTS_CHECK_SIZE(PatternModel, 22528);

} // namespace Xts
#endif // XTS_MODEL_SEQUENCER_PATTERN_MODEL_HPP