#ifndef XTS_DSP_SYNTH_AUDIO_DSP_HPP
#define XTS_DSP_SYNTH_AUDIO_DSP_HPP

#include <DSP/Synth/UnitDSP.hpp>
#include <DSP/Synth/FilterDSP.hpp>
#include <DSP/Synth/AudioState.hpp>
#include <Model/Synth/Config.hpp>

namespace Xts {

class AudioDSP
{
  AudioState _output;
  UnitDSP _units[XTS_SYNTH_UNIT_COUNT];
  FilterDSP _filters[XTS_SYNTH_FILTER_COUNT];
public:
  AudioState const& Next(struct CvState const& cv);
  AudioState const& Output() const { return _output; };
public:
  AudioDSP() = default;
  AudioDSP(AudioModel const* model, int octave, UnitNote note, float rate);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_AUDIO_DSP_HPP