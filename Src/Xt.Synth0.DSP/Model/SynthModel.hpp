#ifndef XTS_SYNTH_MODEL_HPP
#define XTS_SYNTH_MODEL_HPP

#include "TrackConstants.hpp"

namespace Xts {

enum class UnitWave { Saw, Pulse, Tri };
enum class UnitType { Off, Sin, Naive, BasicAdd };
enum class UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };

struct XTS_ALIGN Param { int min, max; int* value; };
struct XTS_ALIGN GlobalModel { int bpm, amp, plot, pad__; };
struct XTS_ALIGN UnitModel { int type, wave, amp, oct, note, cent, basicAddlogParts, pad__; };

struct XTS_ALIGN SynthModel
{
  GlobalModel global;
  UnitModel units[TrackConstants::UnitCount];
  Param params[TrackConstants::ParamCount];
};

} // namespace Xts
#endif // XTS_SYNTH_MODEL_HPP