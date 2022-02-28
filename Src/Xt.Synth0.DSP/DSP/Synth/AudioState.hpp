#ifndef XTS_DSP_SYNTH_AUDIO_STATE_HPP
#define XTS_DSP_SYNTH_AUDIO_STATE_HPP

#include <Model/Model.hpp>
#include <DSP/AudioSample.hpp>

namespace Xts {

struct AudioState
{
  FloatSample units[UnitCount];
  FloatSample filters[FilterCount];
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_AUDIO_STATE_HPP