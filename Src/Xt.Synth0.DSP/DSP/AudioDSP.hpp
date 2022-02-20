#ifndef XTS_AUDIO_DSP_HPP
#define XTS_AUDIO_DSP_HPP

#include "UnitDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class AudioDSP
{
  AudioState _output;
  UnitDSP _units[UnitCount];
public:
  AudioState const& Next(CvState const& cv);
  AudioState const& Output() const { return _output; };
public:
  AudioDSP() = default;
  AudioDSP(AudioModel const* model, int oct, UnitNote note, float rate);
};

} // namespace Xts
#endif // XTS_AUDIO_DSP_HPP