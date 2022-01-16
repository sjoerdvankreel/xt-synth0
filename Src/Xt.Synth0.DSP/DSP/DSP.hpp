#ifndef XTS_DSP_HPP
#define XTS_DSP_HPP

#include <cassert>

namespace Xts {

inline float MaxLevel = 0.95f;

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
inline float Sync(int num, int denom, float rate, int bpm)
{ return bpm * rate * num / (60.0f * denom); }
inline float Time(int val, float rate)
{ return static_cast<float>(val * val * rate / 1000.0f); }

inline float Clip(float val, bool& clipped)
{
  clipped = val > 1.0f || val < -1.0f;
  if(val > 1.0f) val = 1.0f;
  if(val < -1.0f) val = -1.0f;
  return val * MaxLevel;
}

inline float Sync(int val, float rate, int bpm)
{
  auto step = static_cast<SyncStep>(val);
  switch(step)
  {
  case SyncStep::Step1_16: return Sync(1, 16, rate, bpm);
  case SyncStep::Step1_8: return Sync(1, 8, rate, bpm);
  case SyncStep::Step3_16: return Sync(3, 16, rate, bpm);
  case SyncStep::Step1_4: return Sync(1, 4, rate, bpm);
  case SyncStep::Step1_3: return Sync(1, 3, rate, bpm);
  case SyncStep::Step3_8: return Sync(3, 8, rate, bpm);
  case SyncStep::Step1_2: return Sync(1, 2, rate, bpm);
  case SyncStep::Step5_8: return Sync(5, 8, rate, bpm);
  case SyncStep::Step2_3: return Sync(2, 3, rate, bpm);
  case SyncStep::Step3_4: return Sync(3, 4, rate, bpm);
  case SyncStep::Step7_8: return Sync(7, 8, rate, bpm);
  case SyncStep::Step15_16: return Sync(15, 16, rate, bpm);
  case SyncStep::Step1_1: return Sync(1, 1, rate, bpm);
  case SyncStep::Step9_8: return Sync(9, 8, rate, bpm);
  case SyncStep::Step5_4: return Sync(5, 4, rate, bpm);
  case SyncStep::Step4_3: return Sync(4, 3, rate, bpm);
  case SyncStep::Step3_2: return Sync(3, 2, rate, bpm);
  case SyncStep::Step5_3: return Sync(5, 3, rate, bpm);
  case SyncStep::Step7_4: return Sync(7, 4, rate, bpm);
  case SyncStep::Step15_8: return Sync(15, 8, rate, bpm);
  case SyncStep::Step2_1: return Sync(2, 1, rate, bpm);
  case SyncStep::Step3_1: return Sync(3, 1, rate, bpm);
  case SyncStep::Step4_1: return Sync(4, 1, rate, bpm);
  case SyncStep::Step5_1: return Sync(5, 1, rate, bpm);
  case SyncStep::Step6_1: return Sync(6, 1, rate, bpm);
  case SyncStep::Step7_1: return Sync(7, 1, rate, bpm);
  case SyncStep::Step8_1: return Sync(8, 1, rate, bpm);
  case SyncStep::Step10_1: return Sync(10, 1, rate, bpm);
  case SyncStep::Step12_1: return Sync(12, 1, rate, bpm);
  case SyncStep::Step16_1: return Sync(16, 1, rate, bpm);
  default: assert(false); return 0.0f;
  };
}

} // namespace Xts
#endif // XTS_DSP_HPP