#ifndef XTS_DSP_SYNTH_AUDIO_STATE_HPP
#define XTS_DSP_SYNTH_AUDIO_STATE_HPP

#include <Model/Model.hpp>
#include <DSP/AudioSample.hpp>
#include <Model/Synth/Config.hpp>

namespace Xts {

struct AudioState
{
  FloatSample units[XTS_SYNTH_UNIT_COUNT];
  FloatSample filters[XTS_SYNTH_FILTER_COUNT];
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_AUDIO_STATE_HPP