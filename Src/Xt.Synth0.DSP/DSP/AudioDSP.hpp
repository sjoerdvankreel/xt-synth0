#ifndef XTS_AUDIO_DSP_HPP
#define XTS_AUDIO_DSP_HPP

#include "UnitDSP.hpp"
#include <DSP/Synth/FilterDSP.hpp>
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class AudioDSP
{
  AudioState _output;
  UnitDSP _units[UnitCount];
  FilterDSP _flts[FilterCount];
public:
  AudioDSP() = default;
  AudioDSP(AudioModel const* model, int oct, UnitNote note, float rate);
public:
  AudioState const& Next(CvState const& cv);
  AudioState const& Output() const { return _output; };
};

} // namespace Xts
#endif // XTS_AUDIO_DSP_HPP