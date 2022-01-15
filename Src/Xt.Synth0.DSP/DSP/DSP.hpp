#ifndef XTS_DSP_HPP
#define XTS_DSP_HPP

namespace Xts {

inline float Level(int val)
{ return static_cast<float>(val / 255.0f); }
inline float Mix01Exclusive(int val)
{ return static_cast<float>(val / 256.0f); }
inline float Mix01Inclusive(int val)
{ return static_cast<float>((val - 1) / 254.0f); }

} // namespace Xts
#endif // XTS_DSP_HPP