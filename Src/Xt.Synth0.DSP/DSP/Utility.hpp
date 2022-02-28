#ifndef XTS_DSP_UTILITY_HPP
#define XTS_DSP_UTILITY_HPP

#include <cmath>
#include <cstdint>

namespace Xts {

inline constexpr float Epsilon = 1e-4f;
inline constexpr float PIF = 3.14159265358979323846f;
inline constexpr double PID = 3.14159265358979323846;

bool
Clip(float& val);

uint64_t
NextPowerOf2(uint64_t x);

inline float
BipolarToUnipolar2(float val)
{ return val + 1.0f; }

inline float 
BipolarToUnipolar1(float val)
{ return (val + 1.0f) * 0.5f; }

inline float 
EpsilonToZero(float val)
{ return -Epsilon <= val && val <= Epsilon? 0.0f: val; }

inline float
MidiNoteFrequency(float note)
{ return 440.0f * std::powf(2.0f, (note - 69.0f) / 12.0f); }

} // namespace Xts
#endif // XTS_DSP_UTILITY_HPP