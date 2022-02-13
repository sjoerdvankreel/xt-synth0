#ifndef XTS_DSP_HPP
#define XTS_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

#include <cmath>
#include <vector>
#include <cstdint>
#include <cassert>
#include <complex>

namespace Xts {

inline float Eps = 1e-3f;
inline float MaxLevel = 0.95f;
inline float PI = static_cast<float>(3.14159265358979323846);

void Fft(
  std::vector<float> const& x,
  std::vector<std::complex<float>>& fft,
  std::vector<std::complex<float>>& scratch);

inline float BiToUni2(float val)
{ return val + 1.0f; }
inline float BiToUni1(float val)
{ return (val + 1.0f) * 0.5f; }
inline float Level(int val)
{ return static_cast<float>(val) / 256.0f; }
inline float Mix(int val)
{ return static_cast<float>(val - 128) / 127.0f; }
inline float EpsToZero(float val)
{ return -Eps <= val && val <= Eps? 0.0f: val; }
inline float TimeF(int val, float rate)
{ return static_cast<float>(val * val * rate / 1000.0f); }
inline int TimeI(int val, float rate)
{ return static_cast<int>(TimeF(val, rate)); }
inline float Freq(float midi)
{ return 440.0f * powf(2.0f, (midi - 69.0f) / 12.0f); }

inline uint64_t
NextPow2(uint64_t x)
{
  uint64_t result = 0;
  if(x == 0) return 0;
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

inline float 
Mod(float val, bool vbip, float mod, bool mbip, float amt)
{
  float range = 0.0f;
  val = EpsToZero(val);
  assert(-1.0f <= amt && amt <= 1.0f);
  assert(vbip || 0.0f <= val && val <= 1.0f);
  assert(mbip || 0.0f <= mod && mod <= 1.0f);
  assert(!vbip || -1.0f <= val && val <= 1.0f);
  assert(!mbip || -1.0f <= mod && mod <= 1.0f);
  if(amt == 0.0f) return val;
  if(!mbip && amt > 0.0f) range = 1.0f - val;
  if(!mbip && !vbip && amt < 0.0f) range = val;
  if(!mbip && vbip && amt < 0.0f) range = 1.0f + val;
  if(mbip && vbip) range = 1.0f - std::fabs(val);
  if(mbip && !vbip) range = 0.5f - std::fabs(val - 0.5f);
  float result = val + mod * amt * range;
  assert(vbip || 0.0f <= result && result <= 1.0f);
  assert(!vbip || -1.0f <= result && result <= 1.0f);
  return result;
}

} // namespace Xts
#endif // XTS_DSP_HPP