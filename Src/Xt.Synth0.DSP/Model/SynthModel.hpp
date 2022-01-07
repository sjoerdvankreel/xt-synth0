#ifndef XTS_SYNTH_MODEL_HPP
#define XTS_SYNTH_MODEL_HPP

#include "TrackConstants.hpp"

namespace Xts {

enum class UnitType { PolyBlep, Additive, Naive };
enum class UnitWave { Sine, Saw, Pulse, Triangle };
enum class UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };

struct XTS_ALIGN Param { int min, max; int* value; };
struct XTS_ALIGN GlobalModel { int bpm, amp, plot, pad__; };
struct XTS_ALIGN UnitModel { int on, type, wave, logParts, amp, oct, note, cent; };

struct XTS_ALIGN SynthModel
{
  GlobalModel global;
  UnitModel units[TrackConstants::UnitCount];
  Param params[TrackConstants::ParamCount];
};

} // namespace Xts
#endif // XTS_SYNTH_MODEL_HPP