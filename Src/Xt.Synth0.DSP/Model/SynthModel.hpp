#ifndef XTS_SYNTH_MODEL_HPP
#define XTS_SYNTH_MODEL_HPP

#include "TrackConstants.hpp"
#include <cstdint>

namespace Xts {

enum class PlotFit { Auto, Rate, Fit };
enum class PlotSource { Global, Unit1, Unit2, Unit3, Env1, Env2 };

enum class SyncStep
{
  Step0, Step1_16, Step1_8, Step3_16, Step1_4, Step1_3, Step3_8,
  Step1_2, Step5_8, Step2_3, Step3_4, Step7_8, Step15_16, Step1_1,
  Step9_8, Step5_4, Step4_3, Step3_2, Step5_3, Step7_4, Step15_8, Step2_1,
  Step3_1, Step4_1, Step5_1, Step6_1, Step7_1, Step8_1, Step10_1, Step12_1, Step16_1
};

struct XTS_ALIGN GlobalModel { int bpm, env1; };
struct XTS_ALIGN PlotModel { int source, fit; };
struct XTS_ALIGN AutoParam { int min, max; int* value; };

enum class EnvType { Off, DAHDR, DAHDSR };
struct XTS_ALIGN EnvModel 
{
  friend class EnvDSP;
  EnvModel() = default;
  EnvModel(EnvModel const&) = delete;
private:
  EnvType type;
  XtsBool sync;
  int aSlp, dSlp, rSlp;
  int dly, a, hld, d, s, r;
  int dlySnc, aSnc, hldSnc, dSnc, rSnc;
};

enum class NaiveType { Saw, Pulse, Tri };
enum class UnitType { Off, Sin, Naive, Add };
enum class UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };
enum class AddType { Saw, Sqr, Pulse, Tri, Impulse, SinAddSin, SinAddCos, SinSubSin, SinSubCos };
struct XTS_ALIGN UnitModel
{
  friend class UnitDSP;
  UnitModel() = default;
  UnitModel(UnitModel const&) = delete;
private:
  UnitType type;
  UnitNote note;
  AddType addType;
  NaiveType naiveType;
  int32_t amp, pan, oct, dtn, pw;
  int32_t addMaxParts, addParts, addStep, addRoll, pad__;
};

struct XTS_ALIGN SynthModel
{
  PlotModel plot;
  GlobalModel global;
  EnvModel envs[TrackConstants::EnvCount];
  UnitModel units[TrackConstants::UnitCount];
  AutoParam autoParams[TrackConstants::AutoParamCount];
};

} // namespace Xts
#endif // XTS_SYNTH_MODEL_HPP