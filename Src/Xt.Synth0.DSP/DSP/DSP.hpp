#ifndef XTS_DSP_HPP
#define XTS_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

#include <cmath>
#include <vector>
#include <cassert>
#include <complex>

namespace Xts {

inline float MaxLevel = 0.95f;
inline float PI = static_cast<float>(3.14159265358979323846);
void Fft(std::vector<float>& x, std::vector<std::complex<float>>& scratch);

inline float Mix01Exclusive(int val)
{ return static_cast<float>(val / 256.0f); }
inline float Mix02Exclusive(int val)
{ return static_cast<float>(val / 128.0f); }

inline int Mix0100Inclusive(int val)
{ return static_cast<int>((val - 1) / 2.54f); }
inline float Mix01Inclusive(int val)
{ return static_cast<float>((val - 1) / 254.0f); }
inline float Mix02Inclusive(int val)
{ return static_cast<float>((val - 1) / 127.0f); }

inline int Exp(int val)
{ return 1 << val; }
inline float Level(int val)
{ return static_cast<float>(val / 255.0f); }
inline float TimeF(int val, float rate)
{ return static_cast<float>(val * val * rate / 1000.0f); }
inline int TimeI(int val, float rate)
{ return static_cast<int>(TimeF(val, rate)); }

inline float BasicSaw(float phase)
{ return 1.0f - phase * 2.0f; }
inline float BasicSin(float phase)
{ return sinf(phase * 2.0f * PI); }
inline float BasicSqr(float phase)
{ return phase < 0.5f? 1.0f: -1.0f; }
inline float BasicTri(float phase)
{ return (phase < 0.25f ? phase : phase < 0.75f ? 0.5f - phase : -0.25f + (phase - 0.75f)) * 4.0f; }

inline uint64_t
NextPow2(uint64_t x)
{
  uint64_t result = 0;
  if (x && !(x & (x - 1))) return x;
  while (x != 0) x >>= 1, result++;
  return 1ULL << result;
}

inline bool
Clip(float& val)
{
  if (val > MaxLevel) return val = MaxLevel, true;
  if (val < -MaxLevel) return val = -MaxLevel, true;
  return false;
}

inline float 
SyncF(SourceInput const& input, int val)
{
  auto const& step = SyncSteps()[val];
  float fpb = input.rate * 60.0f / input.bpm;
  return fpb * step.num / step.den;
}

inline int SyncI(SourceInput const& input, int val)
{ return static_cast<int>(SyncF(input, val)); }

} // namespace Xts
#endif // XTS_DSP_HPP