#ifndef XTS_DSP_SHARED_BASIC_WAVE_HPP
#define XTS_DSP_SHARED_BASIC_WAVE_HPP

namespace Xts {

enum class BasicWaveType { Sin, Saw, Sqr, Tri };

float
GenerateBasicWave(BasicWaveType type, double phase);

} // namespace Xts
#endif // XTS_DSP_SHARED_BASIC_WAVE_HPP