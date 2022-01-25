#ifndef XTS_SYNTH_MODEL_HPP
#define XTS_SYNTH_MODEL_HPP

#include "Model.hpp"
#include <cstdint>

namespace Xts {

enum class SyncStep 
{ 
  S0, S1_16, S1_8, S3_16, S1_4, S1_3, S3_8, S1_2, S5_8, S2_3, S3_4, S7_8, S15_16, S1_1, S9_8, 
  S5_4, S4_3, S3_2, S5_3, S7_4, S15_8, S2_1, S3_1, S4_1, S5_1, S6_1, S7_1, S8_1, S10_1, S12_1, S16_1 
};

enum class GlobalAmpLfo { LFO1, LFO2 };
enum class GlobalAmpEnv { Env1, Env2, Env3 };
struct XTS_ALIGN GlobalModel
{
  friend class GlobalDSP;
  GlobalModel() = default;
  GlobalModel(GlobalModel const&) = delete;
private:
  GlobalAmpEnv ampEnv;
  GlobalAmpLfo ampLfo;
  int32_t amp, ampEnvAmt, ampLfoAmt, pad__;
};
XTS_CHECK_SIZE(GlobalModel, 24);

enum class LfoType { Sin, Saw, Sqr, Tri };
struct XTS_ALIGN LfoModel
{
  friend class LfoDSP;
  LfoModel() = default;
  LfoModel(LfoModel const&) = delete;
private:
  XtsBool on, sync, inv, key;
  LfoType type;
  int32_t rate;
  SyncStep step;
  int32_t pad__;
};
XTS_CHECK_SIZE(LfoModel, 32);

enum class EnvType { DAHDSR, DAHDR };
struct XTS_ALIGN EnvModel 
{
  friend class EnvDSP;
  EnvModel() = default;
  EnvModel(EnvModel const&) = delete;
private:
  XtsBool on;
  EnvType type;
  XtsBool sync;
  int32_t aSlp, dSlp, rSlp;
  int32_t dly, a, hld, d, s, r;
  SyncStep dlySnc, aSnc, hldSnc, dSnc, rSnc;
  int32_t pad__;
};
XTS_CHECK_SIZE(EnvModel, 72);

enum class PlotHold { Hold, Touch, Hold1Ms, Hold10Ms, Hold100Ms, Hold1S, Hold10S };
enum class PlotType { Off, SynthL, SynthR, Unit1, Unit2, Unit3, Env1, Env2, Env3, LFO1, LFO2 };
struct XTS_ALIGN PlotModel
{
  friend class PlotDSP;
  PlotModel() = default;
  PlotModel(PlotModel const&) = delete;
private:
  PlotType type;
  PlotHold hold;
};
XTS_CHECK_SIZE(PlotModel, 8);

enum class UnitType { Sin, Naive, Add };
enum class NaiveType { Saw, Pulse, Tri };
enum class ModSource { Off, Env1, Env2, Env3, LFO1, LFO2 };
enum class ModTarget { Off, Pitch, Amp, Pan, Dtn, Pw, Roll };
enum class UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };
enum class AddType { Saw, Sqr, Pulse, Tri, Impulse, SinAddSin, SinAddCos, SinSubSin, SinSubCos };
struct XTS_ALIGN UnitModel
{
  friend class UnitDSP;
  UnitModel() = default;
  UnitModel(UnitModel const&) = delete;
private:
  XtsBool on;
  UnitType type;
  UnitNote note;
  AddType addType;
  NaiveType naiveType;
  int32_t amp, pan, oct, dtn, pw;
  int32_t src1, tgt1, amt1, src2, tgt2, amt2;
  int32_t addMaxParts, addParts, addStep, addRoll;
};
XTS_CHECK_SIZE(UnitModel, 80);

struct XTS_ALIGN SynthModel
{
  friend class PlotDSP;
  friend class SynthDSP;
  SynthModel() = default;
  SynthModel(SynthModel const&) = delete;
private:
  PlotModel plot;
  GlobalModel global;
  LfoModel lfos[LfoCount];
  EnvModel envs[EnvCount];
  UnitModel units[UnitCount];
};
XTS_CHECK_SIZE(SynthModel, 552);

} // namespace Xts
#endif // XTS_SYNTH_MODEL_HPP