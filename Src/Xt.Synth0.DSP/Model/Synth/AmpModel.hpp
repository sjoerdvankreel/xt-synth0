#ifndef XTS_MODEL_SYNTH_AMP_MODEL_HPP
#define XTS_MODEL_SYNTH_AMP_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Synth/ModModel.hpp>

namespace Xts {

enum class AmpLfoSource { LFO1, LFO2, LFO3 };
enum class AmpEnvSource { Env1, Env2, Env3 };

struct XTS_ALIGN AmpModel
{
  int32_t panning;
  int32_t panModAmount;
  ModSource panModSource;
  int32_t pad__;

  int32_t amp;
  int32_t ampLfoAmount;
  AmpLfoSource ampLfoSource;
  AmpEnvSource ampEnvSource;

  int32_t unitAmount[XTS_SYNTH_UNIT_COUNT];
  int32_t filterAmount[XTS_SYNTH_FILTER_COUNT];
};
XTS_CHECK_SIZE(AmpModel, 56);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_AMP_MODEL_HPP