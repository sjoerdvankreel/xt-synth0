#ifndef XTS_SYNTH_MODEL_HPP
#define XTS_SYNTH_MODEL_HPP

#include "Model.hpp"
#include <cstdint>

namespace Xts {

struct SyncStep* SyncSteps();
struct ParamInfo* ParamInfos();

void SynthModelInit(
  struct ParamInfo* infos, int32_t infoCount,
  struct SyncStep* steps, int32_t stepCount);

struct XTS_ALIGN SyncStep { int32_t num, den; };
XTS_CHECK_SIZE(SyncStep, 8);
struct XTS_ALIGN ParamInfo { int32_t min, max; };
XTS_CHECK_SIZE(ParamInfo, 8);
struct XTS_ALIGN VoiceBinding { int32_t* params[ParamCount]; };
XTS_CHECK_SIZE(VoiceBinding, 1056);

enum class PlotType { Off, Env1, Env2, Env3, LFO1, LFO2, Unit1, Unit2, Unit3, Global, SynthL, SynthR };
struct XTS_ALIGN PlotModel
{
  friend class PlotDSP;
  PlotModel() = default;
  PlotModel(PlotModel const&) = delete;
private:
  XtsBool spec;
  PlotType type;
  int32_t hold, pad__;
};
XTS_CHECK_SIZE(PlotModel, 16);

enum class GlobalAmpLfo { LFO1, LFO2 };
enum class GlobalAmpEnv { Env1, Env2, Env3 };
struct XTS_ALIGN GlobalModel
{
  friend class GlobalDSP;
  GlobalModel() = default;
  GlobalModel(GlobalModel const&) = delete;
private:
  GlobalAmpEnv envSrc;
  GlobalAmpLfo lfoSrc;
  int32_t amp, lfoAmt;
};
XTS_CHECK_SIZE(GlobalModel, 16);

enum class LfoType { Sin, Saw, Sqr, Tri };
struct XTS_ALIGN LfoModel
{
  friend class LfoDSP;
  friend class GlobalDSP;
  LfoModel() = default;
  LfoModel(LfoModel const&) = delete;
private:
  LfoType type;
  XtsBool on, sync, inv, bip;
  int32_t rate, step, pad__;
};
XTS_CHECK_SIZE(LfoModel, 32);

enum class EnvType { DAHDSR, DAHDR };
enum class SlopeType { Lin, Log, Inv };
struct XTS_ALIGN EnvModel 
{
  friend class EnvDSP;
  EnvModel() = default;
  EnvModel(EnvModel const&) = delete;
private:
  EnvType type;
  XtsBool on, sync, inv;
  SlopeType aSlp, dSlp, rSlp;
  int32_t dly, a, hld, d, s, r;
  int32_t dlyStp, aStp, hldStp, dStp, rStp;
};
XTS_CHECK_SIZE(EnvModel, 72);

enum class UnitType { Sin, Add, Blep };
enum class BlepType { Saw, Pulse, Tri };
enum class ModSource { Velo, LFO1, LFO2, Env1, Env2, Env3 };
enum class ModTarget { Amp, Pan, Pw, Roll, Freq, Pitch, Phase };
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
  XtsBool addSub;
  BlepType blepType;
  int32_t amt1, amt2;
  ModSource src1, src2;
  ModTarget tgt1, tgt2;
  int32_t amp, pan, oct, dtn, pw;
  int32_t addParts, addStep, addRoll, pad__;
};
XTS_CHECK_SIZE(UnitModel, 80);

struct XTS_ALIGN SourceModel
{
  friend class PlotDSP;
  friend class GlobalDSP;
  friend class SourceDSP;
  SourceModel() = default;
  SourceModel(SourceModel const&) = delete;
private:
  LfoModel lfos[LfoCount];
  EnvModel envs[EnvCount];
};
XTS_CHECK_SIZE(SourceModel, 280);

struct XTS_ALIGN SynthModel
{
  friend class SeqDSP;
  friend class PlotDSP;
  friend class SynthDSP;
  SynthModel() = default;
  SynthModel(SynthModel const&) = delete;
private:
  PlotModel plot;
  GlobalModel global;
  SourceModel source;
  UnitModel units[UnitCount];
};
XTS_CHECK_SIZE(SynthModel, 552);

} // namespace Xts
#endif // XTS_SYNTH_MODEL_HPP