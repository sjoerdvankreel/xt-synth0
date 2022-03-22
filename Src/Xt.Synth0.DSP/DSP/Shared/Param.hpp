#ifndef XTS_DSP_SHARED_PARAM_HPP
#define XTS_DSP_SHARED_PARAM_HPP

#include <Model/Shared/SyncStepModel.hpp>
#include <cmath>

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
TimeSamplesF(int val, float rate, float minMs, float maxMs)
{ return (minMs + (maxMs - minMs) * (val / 255.0f) * (val / 255.0f)) * rate / 1000.0f; }

inline int
TimeSamplesI(int val, float rate, float minMs, float maxMs)
{ return static_cast<int>(std::roundf(TimeSamplesF(val, rate, minMs, maxMs))); }

inline float
StepSamplesF(int val, int lpb, float bpm, float rate)
{ return rate * 60.0f * static_cast<float>(lpb / 4.0f) * SyncStepModel::Steps()[val].numerator / SyncStepModel::Steps()[val].denominator * bpm; }

inline int
StepSamplesI(int val, int lpb, float bpm, float rate)
{ return static_cast<int>(StepSamplesF(val, lpb, bpm, rate)); }

inline float
SamplesF(XtsBool sync, int timeVal, int stepVal, int lpb, float bpm, float rate, float minMs, float maxMs)
{ return sync == 0? TimeSamplesF(timeVal, rate, minMs, maxMs): StepSamplesF(stepVal, lpb, bpm, rate); }

inline int
SamplesI(XtsBool sync, int timeVal, int stepVal, int lpb, float bpm, float rate, float minMs, float maxMs)
{ return static_cast<int>(SamplesF(sync, timeVal, stepVal, lpb, bpm, rate, minMs, maxMs)); }

} // namespace Param
} // namespace Xts
#endif // XTS_DSP_SHARED_PARAM_HPP