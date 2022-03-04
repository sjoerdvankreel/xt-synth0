#ifndef XTS_DSP_PARAM_HPP
#define XTS_DSP_PARAM_HPP

#include <Model/SynthModel.hpp>
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
StepSamplesF(int val, float bpm, float rate)
{ return rate * 60.0f / bpm * SyncStepModel::Steps()[val].numerator / SyncStepModel::Steps()[val].denominator; }

inline int
StepSamplesI(int val, float bpm, float rate)
{ return static_cast<int>(StepSamplesF(val, bpm, rate)); }

inline float
SamplesF(XtsBool sync, int timeVal, int stepVal, float bpm, float rate, float minMs, float maxMs)
{ return sync == 0? TimeSamplesF(timeVal, rate, minMs, maxMs): StepSamplesF(stepVal, bpm, rate); }

inline int
SamplesI(XtsBool sync, int timeVal, int stepVal, float bpm, float rate, float minMs, float maxMs)
{ return static_cast<int>(SamplesF(sync, timeVal, stepVal, bpm, rate, minMs, maxMs)); }

} // namespace Param
} // namespace Xts
#endif // XTS_DSP_PARAM_HPP