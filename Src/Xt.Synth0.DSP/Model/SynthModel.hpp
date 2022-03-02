#ifndef XTS_SYNTH_MODEL_HPP
#define XTS_SYNTH_MODEL_HPP

#include <Model/Synth/CvModel.hpp>
#include <Model/Synth/ModModel.hpp>
#include <Model/Synth/LfoModel.hpp>
#include <Model/Synth/PlotModel.hpp>
#include <Model/Synth/UnitModel.hpp>
#include <Model/Synth/AudioModel.hpp>
#include <Model/Synth/FilterModel.hpp>
#include <Model/Synth/EnvelopeModel.hpp>
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
XTS_CHECK_SIZE(VoiceBinding, 1648);

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

struct XTS_ALIGN SynthModel
{
  SynthModel() = default;
  SynthModel(SynthModel const&) = delete;

  CvModel cv;
  AmpModel amp;
  PlotModel plot;
  AudioModel audio;
};
XTS_CHECK_SIZE(SynthModel, 888);

} // namespace Xts
#endif // XTS_SYNTH_MODEL_HPP