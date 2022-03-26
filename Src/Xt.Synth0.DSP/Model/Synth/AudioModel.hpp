#ifndef XTS_MODEL_SYNTH_AUDIO_MODEL_HPP
#define XTS_MODEL_SYNTH_AUDIO_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Synth/UnitModel.hpp>
#include <Model/Synth/SynthConfig.hpp>
#include <Model/Synth/VoiceFilterModel.hpp>

namespace Xts {

struct XTS_ALIGN AudioModel
{
  UnitModel units[XTS_VOICE_UNIT_COUNT];
  VoiceFilterModel filters[XTS_VOICE_FILTER_COUNT];
};
XTS_CHECK_SIZE(AudioModel, 456);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_AUDIO_MODEL_HPP