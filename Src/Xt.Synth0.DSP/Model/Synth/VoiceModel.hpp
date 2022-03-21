#ifndef XTS_MODEL_SYNTH_VOICE_MODEL_HPP
#define XTS_MODEL_SYNTH_VOICE_MODEL_HPP

#include <Model/Synth/CvModel.hpp>
#include <Model/Synth/AmpModel.hpp>
#include <Model/Synth/AudioModel.hpp>
#include <Model/Shared/ParamInfo.hpp>

namespace Xts {

struct XTS_ALIGN VoiceModel
{
  CvModel cv;
  AmpModel amp;
  AudioModel audio;
};
XTS_CHECK_SIZE(VoiceModel, 912);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_VOICE_MODEL_HPP