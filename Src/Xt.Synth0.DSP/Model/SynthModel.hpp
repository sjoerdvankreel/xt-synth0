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

enum class ModSource { Velo, Env1, Env2, Env3, LFO1, LFO2, LFO3 };
struct XTS_ALIGN ModulationModel
{
  int32_t amount;
  int32_t target;
  ModSource source;
  int32_t pad__;
};
XTS_CHECK_SIZE(ModulationModel, 16);

struct XTS_ALIGN SyncStep { int32_t num, den; };
XTS_CHECK_SIZE(SyncStep, 8);
struct XTS_ALIGN ParamInfo { int32_t min, max; };
XTS_CHECK_SIZE(ParamInfo, 8);
struct XTS_ALIGN VoiceBinding { int32_t* params[ParamCount]; };
XTS_CHECK_SIZE(VoiceBinding, 1648);

enum class PlotType { Synth, Amp, Env1, Env2, Env3, LFO1, LFO2, LFO3, Unit1, Unit2, Unit3, Filt1, Filt2, Filt3 };
struct XTS_ALIGN PlotModel
{
  friend class PlotDSP;
  PlotModel() = default;
  PlotModel(PlotModel const&) = delete;
private:
  XtsBool on, spec;
  PlotType type;
  int32_t hold;
};
XTS_CHECK_SIZE(PlotModel, 16);

enum class LfoType { Sin, Saw, Sqr, Tri };
enum class LfoPolarity { Uni, UniInv, Bi, BiInv };
struct XTS_ALIGN LfoModel
{
  friend class LfoDSP;
  LfoModel() = default;
  LfoModel(LfoModel const&) = delete;
private:
  LfoType type;
  LfoPolarity plty;
  XtsBool on, sync;
  int32_t prd, step;
};
XTS_CHECK_SIZE(LfoModel, 24);

enum class EnvType { DAHDSR, DAHDR };
enum class SlopeType { Lin, Log, Inv, Sin, Cos };
struct XTS_ALIGN EnvModel
{
  friend class EnvDSP;
  friend class PlotDSP;
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

enum class FilterType { Bqd, Comb };
enum class BiquadType { LPF, HPF, BPF, BSF };
enum class FilterModTarget { Freq, Res, GPlus, DlyPlus, GMin, DlyMin };
struct XTS_ALIGN FilterModel
{
  FilterModel() = default;
  FilterModel(FilterModel const&) = delete;

  XtsBool on;
  FilterType type;

  int32_t combMinGain;
  int32_t combPlusGain;
  int32_t combMinDelay;
  int32_t combPlusDelay;

  BiquadType biquadType;
  int32_t biquadResonance;
  int32_t biquadFrequency;
  int32_t pad__;

  ModulationModel modulation1;
  ModulationModel modulation2;
  int32_t unitAmount[UnitCount];
  int32_t filterAmount[FilterCount];
};
XTS_CHECK_SIZE(FilterModel, 96);

enum class UnitType { Sin, Add, Blep };
enum class BlepType { Saw, Pulse, Tri };
enum class UnitModTarget { Amp, Pan, Pw, Roll, Freq, Pitch, Phase };
enum class UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };
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
  UnitModTarget tgt1, tgt2;
  int32_t amp, pan, oct, dtn, pw;
  int32_t addParts, addStep, addRoll, pad__;
};
XTS_CHECK_SIZE(UnitModel, 80);

enum class AmpLfo { LFO1, LFO2, LFO3 };
enum class AmpEnv { Env1, Env2, Env3 };
struct XTS_ALIGN AmpModel
{
  friend class AmpDSP;
  friend class PlotDSP;
  AmpModel() = default;
  AmpModel(AmpModel const&) = delete;
private:
  AmpEnv envSrc;
  AmpLfo lvlSrc;
  ModSource panSrc;
  int32_t units[UnitCount];
  int32_t flts[FilterCount];
  int32_t lvl, pan, lvlAmt, panAmt, pad__;
};
XTS_CHECK_SIZE(AmpModel, 56);

struct XTS_ALIGN CvModel
{
  friend class CvDSP;
  friend class PlotDSP;
  CvModel() = default;
  CvModel(CvModel const&) = delete;
private:
  LfoModel lfos[LfoCount];
  EnvModel envs[EnvCount];
};
XTS_CHECK_SIZE(CvModel, 288);

struct XTS_ALIGN AudioModel
{
  friend class PlotDSP;
  friend class AudioDSP;
  AudioModel() = default;
  AudioModel(AudioModel const&) = delete;
private:
  UnitModel units[UnitCount];
  FilterModel filts[FilterCount];
};
XTS_CHECK_SIZE(AudioModel, 528);

struct XTS_ALIGN SynthModel
{
  friend class PlotDSP;
  friend class SynthDSP;
  SynthModel() = default;
  SynthModel(SynthModel const&) = delete;
private:
  CvModel cv;
  AmpModel amp;
  PlotModel plot;
  AudioModel audio;
};
XTS_CHECK_SIZE(SynthModel, 888);

} // namespace Xts
#endif // XTS_SYNTH_MODEL_HPP