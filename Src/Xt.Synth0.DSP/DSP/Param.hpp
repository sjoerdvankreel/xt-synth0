#ifndef XTS_DSP_PARAM_HPP
#define XTS_DSP_PARAM_HPP

#include <Model/SynthModel.hpp>

namespace Xts { namespace Param {

inline float 
Level(int val)
{ return static_cast<float>(val) / 256.0f; }

inline float
Mix(int val)
{ return static_cast<float>(val - 128) / 127.0f; }

inline float
Frequency(int val)
{ return 20.0f + 9980.0f * (val / 255.0f) * (val / 255.0f); }

inline float
StepFramesF(float bpm, float rate, int val)
{ return rate * 60.0f / bpm * SyncSteps()[val].num / SyncSteps()[val].den; }

inline int 
StepFramesI(float bpm, float rate, int val)
{ return static_cast<int>(StepFramesF(bpm, rate, val)); }

inline float 
TimeFramesF(int val, float rate)
{ return static_cast<float>((val / 2.55f) * (val / 2.55f) * rate / 1000.0f); }

inline int 
TimeFramesI(int val, float rate)
{ return static_cast<int>(TimeFramesF(val, rate)); }

} // namespace Param
} // namespace Xts
#endif // XTS_DSP_PARAM_HPP