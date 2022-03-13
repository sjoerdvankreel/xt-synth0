#ifndef XTS_DSP_SYNTH_AUDIO_STATE_HPP
#define XTS_DSP_SYNTH_AUDIO_STATE_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Synth/SynthConfig.hpp>
#include <DSP/Shared/AudioSample.hpp>

namespace Xts {

struct AudioState
{
  FloatSample units[XTS_SYNTH_UNIT_COUNT];
  FloatSample filters[XTS_SYNTH_FILTER_COUNT];
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_AUDIO_STATE_HPP