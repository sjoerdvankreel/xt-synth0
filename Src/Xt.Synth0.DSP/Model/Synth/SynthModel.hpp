#ifndef XTS_MODEL_SYNTH_SYNTH_MODEL_HPP
#define XTS_MODEL_SYNTH_SYNTH_MODEL_HPP

#include <Model/Synth/CvModel.hpp>
#include <Model/Synth/AmpModel.hpp>
#include <Model/Synth/PlotModel.hpp>
#include <Model/Synth/AudioModel.hpp>
#include <Model/Shared/ParamInfo.hpp>

#include <cstdint>

namespace Xts {

struct XTS_ALIGN SynthModel
{
  CvModel cv;
  AmpModel amp;
  PlotModel plot;
  AudioModel audio;
public:
  static ParamInfo const* Params();
  static void Init(ParamInfo* params, size_t count);
};
XTS_CHECK_SIZE(SynthModel, 936);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_SYNTH_MODEL_HPP