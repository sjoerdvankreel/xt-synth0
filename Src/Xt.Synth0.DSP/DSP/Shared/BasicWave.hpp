#ifndef XTS_DSP_SHARED_BASIC_WAVE_HPP
#define XTS_DSP_SHARED_BASIC_WAVE_HPP

namespace Xts {

enum class BasicWaveType { Sine, Saw, Square, Triangle };

float
GenerateBasicWave(BasicWaveType type, float phase);

} // namespace Xts
#endif // XTS_DSP_SHARED_BASIC_WAVE_HPP