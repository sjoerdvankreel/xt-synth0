#ifndef XTS_SYNTH_MODEL_HPP
#define XTS_SYNTH_MODEL_HPP

namespace Xts {

struct AmpModel { int a, d, s, r, lvl; };
struct Param { int min, max; int* value; };
struct GlobalModel { int bpm, hmns, plot, method; };
struct UnitModel { int on, amp, oct, note, cent, type; };

enum class SynthMethod { PBP, Add, Nve };
enum class UnitType { Sin, Saw, Sqr, Tri };
enum class UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };

struct SynthModel
{
  static constexpr int UnitCount = 3;
  static constexpr int ParamCount = 1;
  AmpModel amp;
  GlobalModel global;
  UnitModel units[UnitCount];
  Param params[ParamCount];
};

} // namespace Xts
#endif // XTS_SYNTH_MODEL_HPP