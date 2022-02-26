#ifndef XTS_DSP_HPP
#define XTS_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

#include <cmath>
#include <string>
#include <vector>
#include <cstdint>
#include <cassert>
#include <complex>

namespace Xts {

inline float Eps = 1e-3f;
inline float PI = static_cast<float>(3.14159265358979323846);

extern std::vector<VSplit> BiVSPlits;
extern std::vector<VSplit> UniVSPlits;
extern std::vector<VSplit> StereoVSPlits;
std::vector<VSplit> MakeBiVSplits(float max);

float Modulate(float val, bool bip, float amt, CvOutput cv);
CvOutput ModulationInput(CvState const& cv, ModSource src);
ModInput ModulationInput(CvState const& cv, ModSource src1, ModSource src2);

std::wstring FormatEnv(EnvStage stage);
void Fft(std::vector<float> const& x, std::vector<std::complex<float>>& fft, std::vector<std::complex<float>>& scratch);

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
inline float FreqNote(float midi)
{ return 440.0f * powf(2.0f, (midi - 69.0f) / 12.0f); }
inline float FreqHz(int val)
{ return 10.0f + 9990.0f * (val / 255.0f) * (val / 255.0f); }
inline float TimeF(int val, float rate)
{ return static_cast<float>((val / 2.55f) * (val / 2.55f) * rate / 1000.0f); }
inline int TimeI(int val, float rate)
{ return static_cast<int>(TimeF(val, rate)); }

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

inline float 
SyncF(float bpm, float rate, int val)
{
  auto const& step = SyncSteps()[val];
  float fpb = rate * 60.0f / bpm;
  return fpb * step.num / step.den;
}

inline int SyncI(float bpm, float rate, int val)
{ return static_cast<int>(SyncF(bpm, rate, val)); }

} // namespace Xts
#endif // XTS_DSP_HPP