#ifndef XTS_SYNTH_MODEL_HPP
#define XTS_SYNTH_MODEL_HPP

#include "TrackConstants.hpp"

namespace Xts {

enum class PlotFit { Auto, Rate, Fit };
enum class NaiveType { Saw, Pulse, Tri };
enum class AmpEnvSource { Off, Env1, Env2 };
enum class UnitType { Off, Sin, Naive, Add };
enum class PlotSource { Unit1, Unit2, Unit3, Env1, Env2 };
enum class UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };
enum class AddType { Saw, Sqr, Pulse, Tri, Impulse, SinAddSin, SinAddCos, SinSubSin, SinSubCos };

struct XTS_ALIGN PlotModel { int source, fit; };
struct XTS_ALIGN AutoParam { int min, max; int* value; };
struct XTS_ALIGN GlobalModel { int bpm, amp, ampSrc, ampAmt; };

struct XTS_ALIGN EnvModel {
  int a, d, s, r, hld, dly;
  int on, aSlp, dSlp, rSlp;
};

struct XTS_ALIGN UnitModel {
  int type, naiveType, amp, pan, oct, note, dtn, pw;
  int addType, addParts, addMaxParts, addStep, addRoll, pad__;
};

struct XTS_ALIGN SynthModel
{
  PlotModel plot;
  GlobalModel global;
  UnitModel units[TrackConstants::UnitCount];
  EnvModel envs[TrackConstants::EnvCount];
  AutoParam autoParams[TrackConstants::AutoParamCount];
};

} // namespace Xts
#endif // XTS_SYNTH_MODEL_HPP