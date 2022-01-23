#ifndef XTS_DSP_HPP
#define XTS_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

#include <cassert>

namespace Xts {

inline float MaxLevel = 0.95f;
inline float PI = static_cast<float>(3.14159265358979323846);

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
inline float SyncF(SynthInput const& input, int num, int denom)
{ return static_cast<float>(input.bpm * input.rate * num / (60.0f * denom)); }
inline int SyncI(SynthInput const& input, int num, int denom)
{ return static_cast<int>(SyncF(input, num, denom)); }

inline bool 
Clip(float& val)
{
  if(val > MaxLevel) return val = MaxLevel, true;
  if(val < -MaxLevel) return val = -MaxLevel, true;
  return false;
}

inline float 
SyncF(SynthInput const& input, int val)
{
  auto step = static_cast<SyncStep>(val);
  switch(step)
  {
  case SyncStep::S0: return 0;
  case SyncStep::S1_16: return SyncF(input, 1, 16);
  case SyncStep::S1_8: return SyncF(input, 1, 8);
  case SyncStep::S3_16: return SyncF(input, 3, 16);
  case SyncStep::S1_4: return SyncF(input, 1, 4);
  case SyncStep::S1_3: return SyncF(input, 1, 3);
  case SyncStep::S3_8: return SyncF(input, 3, 8);
  case SyncStep::S1_2: return SyncF(input, 1, 2);
  case SyncStep::S5_8: return SyncF(input, 5, 8);
  case SyncStep::S2_3: return SyncF(input, 2, 3);
  case SyncStep::S3_4: return SyncF(input, 3, 4);
  case SyncStep::S7_8: return SyncF(input, 7, 8);
  case SyncStep::S15_16: return SyncF(input, 15, 16);
  case SyncStep::S1_1: return SyncF(input, 1, 1);
  case SyncStep::S9_8: return SyncF(input, 9, 8);
  case SyncStep::S5_4: return SyncF(input, 5, 4);
  case SyncStep::S4_3: return SyncF(input, 4, 3);
  case SyncStep::S3_2: return SyncF(input, 3, 2);
  case SyncStep::S5_3: return SyncF(input, 5, 3);
  case SyncStep::S7_4: return SyncF(input, 7, 4);
  case SyncStep::S15_8: return SyncF(input, 15, 8);
  case SyncStep::S2_1: return SyncF(input, 2, 1);
  case SyncStep::S3_1: return SyncF(input, 3, 1);
  case SyncStep::S4_1: return SyncF(input, 4, 1);
  case SyncStep::S5_1: return SyncF(input, 5, 1);
  case SyncStep::S6_1: return SyncF(input, 6, 1);
  case SyncStep::S7_1: return SyncF(input, 7, 1);
  case SyncStep::S8_1: return SyncF(input, 8, 1);
  case SyncStep::S10_1: return SyncF(input, 10, 1);
  case SyncStep::S12_1: return SyncF(input, 12, 1);
  case SyncStep::S16_1: return SyncF(input, 16, 1);
  default: assert(false); return 0;
  };
}
inline int
SyncI(SynthInput const& input, int val)
{ return static_cast<int>(SyncF(input, val)); }

} // namespace Xts
#endif // XTS_DSP_HPP