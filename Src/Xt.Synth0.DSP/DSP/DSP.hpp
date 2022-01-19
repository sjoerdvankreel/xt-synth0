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

inline float Mix01Inclusive(int val)
{ return static_cast<float>((val - 1) / 254.0f); }
inline float Mix02Inclusive(int val)
{ return static_cast<float>((val - 1) / 127.0f); }
inline float Mix0100Inclusive(int val)
{ return static_cast<float>((val - 1) / 2.54f); }

inline int Exp(int val)
{ return 1 << val; }
inline float Level(int val)
{ return static_cast<float>(val / 255.0f); }
inline float Time(int val, float rate)
{ return static_cast<float>(val * val * rate / 1000.0f); }
inline float Sync(AudioInput const& input, int num, int denom)
{ return input.bpm * input.rate * num / (60.0f * denom); }

inline float 
Clip(float val, bool& clipped)
{
  clipped = val > 1.0f || val < -1.0f;
  if(val > 1.0f) val = 1.0f;
  if(val < -1.0f) val = -1.0f;
  return val * MaxLevel;
}

inline float 
Sync(AudioInput const& input, int val)
{
  auto step = static_cast<SyncStep>(val);
  switch(step)
  {
  case SyncStep::Step0: return 0.0f;
  case SyncStep::Step1_16: return Sync(input, 1, 16);
  case SyncStep::Step1_8: return Sync(input, 1, 8);
  case SyncStep::Step3_16: return Sync(input, 3, 16);
  case SyncStep::Step1_4: return Sync(input, 1, 4);
  case SyncStep::Step1_3: return Sync(input, 1, 3);
  case SyncStep::Step3_8: return Sync(input, 3, 8);
  case SyncStep::Step1_2: return Sync(input, 1, 2);
  case SyncStep::Step5_8: return Sync(input, 5, 8);
  case SyncStep::Step2_3: return Sync(input, 2, 3);
  case SyncStep::Step3_4: return Sync(input, 3, 4);
  case SyncStep::Step7_8: return Sync(input, 7, 8);
  case SyncStep::Step15_16: return Sync(input, 15, 16);
  case SyncStep::Step1_1: return Sync(input, 1, 1);
  case SyncStep::Step9_8: return Sync(input, 9, 8);
  case SyncStep::Step5_4: return Sync(input, 5, 4);
  case SyncStep::Step4_3: return Sync(input, 4, 3);
  case SyncStep::Step3_2: return Sync(input, 3, 2);
  case SyncStep::Step5_3: return Sync(input, 5, 3);
  case SyncStep::Step7_4: return Sync(input, 7, 4);
  case SyncStep::Step15_8: return Sync(input, 15, 8);
  case SyncStep::Step2_1: return Sync(input, 2, 1);
  case SyncStep::Step3_1: return Sync(input, 3, 1);
  case SyncStep::Step4_1: return Sync(input, 4, 1);
  case SyncStep::Step5_1: return Sync(input, 5, 1);
  case SyncStep::Step6_1: return Sync(input, 6, 1);
  case SyncStep::Step7_1: return Sync(input, 7, 1);
  case SyncStep::Step8_1: return Sync(input, 8, 1);
  case SyncStep::Step10_1: return Sync(input, 10, 1);
  case SyncStep::Step12_1: return Sync(input, 12, 1);
  case SyncStep::Step16_1: return Sync(input, 16, 1);
  default: assert(false); return 0.0f;
  };
}

} // namespace Xts
#endif // XTS_DSP_HPP