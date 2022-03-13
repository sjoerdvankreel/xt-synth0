#ifndef XTS_MODEL_SEQUENCER_PATTERN_KEY_MODEL_HPP
#define XTS_MODEL_SEQUENCER_PATTERN_KEY_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <cstdint>

namespace Xts {

enum class PatternNote { None, Off, C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };

struct XTS_ALIGN PatternKeyModel
{
  int32_t octave;
  int32_t velocity;
  PatternNote note;
  int32_t pad__;
};
XTS_CHECK_SIZE(PatternKeyModel, 16);

} // namespace Xts
#endif // XTS_MODEL_SEQUENCER_PATTERN_KEY_MODEL_HPP