#ifndef XTS_SYNTH_MODEL_HPP
#define XTS_SYNTH_MODEL_HPP

#include "TrackConstants.hpp"

namespace Xts {

enum class PlotFit { Auto, Rate, Fit };
enum class NaiveType { Saw, Pulse, Tri };
enum class EnvType { Off, DAHDR, DAHDSR };
enum class UnitType { Off, Sin, Naive, Add };
enum class AmpEnv { NoAmpEnv, AmpEnv1, AmpEnv2 };
enum class PlotSource { Global, Unit1, Unit2, Unit3, Env1, Env2 };
enum class UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };
enum class AddType { Saw, Sqr, Pulse, Tri, Impulse, SinAddSin, SinAddCos, SinSubSin, SinSubCos };

enum class SyncStep
{
  Step0, Step1_16, Step1_8, Step3_16, Step1_4, Step1_3, Step3_8,
  Step1_2, Step5_8, Step2_3, Step3_4, Step7_8, Step15_16, Step1_1,
  Step9_8, Step5_4, Step4_3, Step3_2, Step5_3, Step7_4, Step15_8, Step2_1,
  Step3_1, Step4_1, Step5_1, Step6_1, Step7_1, Step8_1, Step10_1, Step12_1, Step16_1
};

struct XTS_ALIGN PlotModel { int source, fit; };
struct XTS_ALIGN AutoParam { int min, max; int* value; };
struct XTS_ALIGN GlobalModel { int env, amp, bpm, pad__; };

struct XTS_ALIGN EnvModel {
  int a, d, s, r, hld, dly;
  int type, sync, aSlp, dSlp, rSlp;
  int hldSnc, dlySnc, aSnc, dSnc, rSnc;
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