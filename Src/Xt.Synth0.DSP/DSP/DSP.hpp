#ifndef XTS_DSP_HPP
#define XTS_DSP_HPP

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
inline float Time(int val, float rate)
{ return static_cast<float>(val * val * rate / 1000.0f); }

inline float Clip(float val, bool& clipped)
{
  clipped = val > 1.0f || val < -1.0f;
  if(val > 1.0f) val = 1.0f;
  if(val < -1.0f) val = -1.0f;
  return val * MaxLevel;
}

} // namespace Xts
#endif // XTS_DSP_HPP