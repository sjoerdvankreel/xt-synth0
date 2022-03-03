#ifndef XTS_SYNTH_MODEL_HPP
#define XTS_SYNTH_MODEL_HPP

#include <Model/Synth/CvModel.hpp>
#include <Model/Synth/ModModel.hpp>
#include <Model/Synth/AmpModel.hpp>
#include <Model/Synth/LfoModel.hpp>
#include <Model/Synth/PlotModel.hpp>
#include <Model/Synth/UnitModel.hpp>
#include <Model/Synth/AudioModel.hpp>
#include <Model/Synth/FilterModel.hpp>
#include <Model/Synth/EnvelopeModel.hpp>
#include <Model/Synth/SyncStepModel.hpp>
#include "Model.hpp"
#include <cstdint>

namespace Xts {

struct ParamInfo* ParamInfos();

void SynthModelInit(
  struct ParamInfo* infos, int32_t infoCount);
struct XTS_ALIGN ParamInfo { int32_t min, max; };
XTS_CHECK_SIZE(ParamInfo, 8);
struct XTS_ALIGN VoiceBinding { int32_t* params[ParamCount]; };
XTS_CHECK_SIZE(VoiceBinding, 1648);

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