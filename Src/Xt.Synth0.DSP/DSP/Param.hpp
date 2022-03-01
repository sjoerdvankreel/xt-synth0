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
Frequency(int val, float minHz, float maxHz)
{ return minHz + (maxHz - minHz) * (val / 255.0f) * (val / 255.0f); }

inline float
StepFramesF(int val, float bpm, float rate)
{ return rate * 60.0f / bpm * SyncSteps()[val].num / SyncSteps()[val].den; }

inline int 
StepFramesI(int val, float bpm, float rate)
{ return static_cast<int>(StepFramesF(val, bpm, rate)); }

inline float 
TimeFramesF(int val, float rate, float minMs, float maxMs)
{ return (minMs + (maxMs - minMs) * (val / 255.0f) * (val / 255.0f)) * rate / 1000.0f; }

inline int 
TimeFramesI(int val, float rate, float minMs, float maxMs)
{ return static_cast<int>(TimeFramesF(val, rate, minMs, maxMs)); }

} // namespace Param
} // namespace Xts
#endif // XTS_DSP_PARAM_HPP