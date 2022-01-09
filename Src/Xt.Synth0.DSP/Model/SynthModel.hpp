#ifndef XTS_SYNTH_MODEL_HPP
#define XTS_SYNTH_MODEL_HPP

#include "TrackConstants.hpp"

namespace Xts {

enum class UnitType { Off, Sine, Naive, Additive };
enum class NaiveType { Saw, Pulse, Triangle, Impulse };
enum class UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };
enum class AdditiveType { Saw, Pulse, Triangle, Impulse, SinAddSin, SinAddCos, SinSubSin, SinSubCos };

struct XTS_ALIGN Param { int min, max; int* value; };
struct XTS_ALIGN GlobalModel { int bpm, amp, plot, pad__; };
struct XTS_ALIGN UnitModel { 
  int type, naiveType, amp, oct, note, cent, pwm;
  int addType, addParts, addMaxParts, addStep, addRolloff;
};

struct XTS_ALIGN SynthModel
{
  GlobalModel global;
  UnitModel units[TrackConstants::UnitCount];
  Param params[TrackConstants::ParamCount];
};

} // namespace Xts
#endif // XTS_SYNTH_MODEL_HPP