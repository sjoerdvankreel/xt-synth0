#ifndef XTS_SYNTH_MODEL_HPP
#define XTS_SYNTH_MODEL_HPP

#include "TrackConstants.hpp"

namespace Xts {

struct XTS_ALIGN Param { int min, max; int* value; };
struct XTS_ALIGN AmpModel { int a, d, s, r, lvl, pad__; };
struct XTS_ALIGN GlobalModel { int bpm, hmns, plot, method; };
struct XTS_ALIGN UnitModel { int on, amp, oct, note, cent, type; };

enum class SynthMethod { PBP, Add, Nve };
enum class UnitType { Sin, Saw, Sqr, Tri };
enum class UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };

struct XTS_ALIGN SynthModel
{
  AmpModel amp;
  GlobalModel global;
  UnitModel units[TrackConstants::UnitCount];
  Param params[TrackConstants::ParamCount];
};

} // namespace Xts
#endif // XTS_SYNTH_MODEL_HPP