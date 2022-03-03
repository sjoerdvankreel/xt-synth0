#ifndef XTS_DSP_UTILITY_HPP
#define XTS_DSP_UTILITY_HPP

#include <cmath>
#include <cassert>
#include <cstdint>

namespace Xts {

inline constexpr float Epsilon = 1e-4f;
inline constexpr float PIF = 3.14159265358979323846f;
inline constexpr double PID = 3.14159265358979323846;

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

inline uint64_t
NextPowerOf2(uint64_t x)
{
  uint64_t result = 0;
  if (x == 0) return 0;
  if (x && !(x & (x - 1))) return x;
  while (x != 0) x >>= 1, result++;
  return 1ULL << result;
}

inline bool
Clip(float& val)
{
  if (val > 1.0f) return val = 1.0f, true;
  if (val < -1.0f) return val = -1.0f, true;
  return false;
}

template <class T>
inline T Sanity(T val)
{
  assert(!std::isnan(val));
  assert(!std::isinf(val));
  assert(std::fpclassify(val) != FP_SUBNORMAL);
  return val;
}

template <class T>
inline T BipolarSanity(T val)
{
  Sanity(val);
  assert(val <= static_cast<T>(1.0));
  assert(val >= static_cast<T>(-1.0));
  return val;
}

template <class T>
inline T UnipolarSanity(T val)
{
  Sanity(val);
  assert(val <= static_cast<T>(1.0));
  assert(val >= static_cast<T>(0.0));
  return val;
}

} // namespace Xts
#endif // XTS_DSP_UTILITY_HPP