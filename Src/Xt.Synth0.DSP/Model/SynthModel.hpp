#ifndef XTS_SYNTH_MODEL_HPP
#define XTS_SYNTH_MODEL_HPP

#include "TrackConstants.hpp"

namespace Xts {

enum class PlotFit { Auto, Rate, Fit };
enum class NaiveType { Saw, Pulse, Tri };
enum class UnitType { Off, Sin, Naive, Add };
enum class SlopeType { Lin, Sqrt, Quad, Log, Exp };
enum class PlotSource { Unit1, Unit2, Unit3, Env1, Env2 };
enum class UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };
enum class AddType { Saw, Sqr, Pulse, Tri, Impulse, SinAddSin, SinAddCos, SinSubSin, SinSubCos };

struct XTS_ALIGN GlobalModel { int bpm, amp; };
struct XTS_ALIGN PlotModel { int source, fit; };
struct XTS_ALIGN Param { int min, max; int* value; };

struct XTS_ALIGN EnvModel {
  int a, d, s, r, hld, dly;
  int on, aSlope, dSlope, rSlope;
};

struct XTS_ALIGN UnitModel {
  int type, naiveType, amp, oct, note, cent, pwm;
  int addType, addParts, addMaxParts, addStep, addRoll;
};

struct XTS_ALIGN SynthModel
{
  PlotModel plot;
  GlobalModel global;
  UnitModel units[TrackConstants::UnitCount];
  EnvModel envs[TrackConstants::EnvCount];
  Param params[TrackConstants::ParamCount];
};

} // namespace Xts
#endif // XTS_SYNTH_MODEL_HPP