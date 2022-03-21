#ifndef XTS_MODEL_SYNTH_AMP_MODEL_HPP
#define XTS_MODEL_SYNTH_AMP_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Synth/ModModel.hpp>

namespace Xts {

enum class AmpLfoSource { LFO1, LFO2 };

struct XTS_ALIGN AmpModel
{
  int32_t panning;
  int32_t panModAmount;
  ModSource panModSource;

  int32_t amp;
  int32_t ampLfoAmount;
  AmpLfoSource ampLfoSource;

  int32_t unitAmount[XTS_VOICE_UNIT_COUNT];
  int32_t filterAmount[XTS_VOICE_FILTER_COUNT];
};
XTS_CHECK_SIZE(AmpModel, 48);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_AMP_MODEL_HPP