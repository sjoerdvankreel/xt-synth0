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

struct XTS_ALIGN GlobalModel
{
  friend class SynthDSP;
  GlobalModel() = default;
  GlobalModel(GlobalModel const&) = delete;
private:
  int32_t amp, env1;
};
XTS_CHECK_SIZE(GlobalModel, 8);

enum class EnvType { Off, DAHDR, DAHDSR };
struct XTS_ALIGN EnvModel 
{
  friend class EnvDSP;
  EnvModel() = default;
  EnvModel(EnvModel const&) = delete;
private:
  EnvType type;
  XtsBool sync;
  int32_t aSlp, dSlp, rSlp;
  int32_t dly, a, hld, d, s, r;
  int32_t dlySnc, aSnc, hldSnc, dSnc, rSnc;
};
XTS_CHECK_SIZE(EnvModel, 64);

enum class PlotType { SynthL, SynthR, Unit1, Unit2, Unit3, Env1, Env2 };
struct XTS_ALIGN PlotModel
{
  friend class PlotDSP;
  PlotModel() = default;
  PlotModel(PlotModel const&) = delete;
private:
  PlotType type;
  int pad__;
};
XTS_CHECK_SIZE(PlotModel, 8);

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
XTS_CHECK_SIZE(UnitModel, 56);

struct XTS_ALIGN SynthModel
{
  friend class PlotDSP;
  friend class SynthDSP;
  SynthModel() = default;
  SynthModel(SynthModel const&) = delete;
private:
  PlotModel plot;
  GlobalModel global;
  EnvModel envs[EnvCount];
  UnitModel units[UnitCount];
};
XTS_CHECK_SIZE(SynthModel, 312);

} // namespace Xts
#endif // XTS_SYNTH_MODEL_HPP