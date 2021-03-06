#ifndef XTS_DSP_SYNTH_AUDIO_DSP_HPP
#define XTS_DSP_SYNTH_AUDIO_DSP_HPP

#include <DSP/Synth/UnitDSP.hpp>
#include <DSP/Synth/AudioState.hpp>
#include <DSP/Synth/VoiceFilterDSP.hpp>
#include <Model/Synth/SynthConfig.hpp>
#include <Model/Shared/NoteType.hpp>

namespace Xts {

class AudioDSP
{
  AudioState _output;
  UnitDSP _units[XTS_VOICE_UNIT_COUNT];
  VoiceFilterDSP _filters[XTS_VOICE_FILTER_COUNT];
public:
  AudioState const& Next(struct CvState const& cv);
  AudioState const& Output() const { return _output; };
public:
  AudioDSP() = default;
  AudioDSP(struct AudioModel const* model, int octave, NoteType note, float rate);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_AUDIO_DSP_HPP