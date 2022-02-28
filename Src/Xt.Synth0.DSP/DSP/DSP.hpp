#ifndef XTS_DSP_HPP
#define XTS_DSP_HPP

#include <DSP/Synth/AudioState.hpp>
#include <DSP/Synth/CvState.hpp>
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

#include <cmath>
#include <cstdint>
#include <cassert>
#include <complex>

namespace Xts {

inline float Eps = 1e-3f;
inline float PI = static_cast<float>(3.14159265358979323846);

float Modulate(float val, bool bip, float amt, CvSample cv);
CvSample ModulationInput(CvState const& cv, ModSource src);
ModInput ModulationInput(CvState const& cv, ModSource src1, ModSource src2);

void Fft(std::vector<float> const& x, std::vector<std::complex<float>>& fft, std::vector<std::complex<float>>& scratch);

inline float BiToUni2(float val)
{ return val + 1.0f; }
inline float BiToUni1(float val)
{ return (val + 1.0f) * 0.5f; }
inline float EpsToZero(float val)
{ return -Eps <= val && val <= Eps? 0.0f: val; }
inline float FreqNote(float midi)
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
  if (val > 1.0f) return val = 1.0f, true;
  if (val < -1.0f) return val = -1.0f, true;
  return false;
}

} // namespace Xts
#endif // XTS_DSP_HPP