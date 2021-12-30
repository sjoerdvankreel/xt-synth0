#ifndef XTS_SYNTH_MODEL_HPP
#define XTS_SYNTH_MODEL_HPP

namespace Xts {

enum class SynthMethod { PBP, Add, Nve };
enum class UnitType { Sin, Saw, Sqr, Tri };
enum class UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp,	B };

struct AmpModel { int a, d, s, r, lvl; };
struct GlobalModel { int bpm, hmns, plot, method; };
struct UnitModel { int on, amp, oct, note, cent, type; };

struct SynthModel
{
  static constexpr int UnitCount = 3;
  AmpModel amp;
  GlobalModel global;
  UnitModel units[UnitCount];
};

} // namespace Xts
#endif // XTS_SYNTH_MODEL_HPP