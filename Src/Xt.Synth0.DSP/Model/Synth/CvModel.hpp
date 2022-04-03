#ifndef XTS_MODEL_SYNTH_CV_MODEL_HPP
#define XTS_MODEL_SYNTH_CV_MODEL_HPP

#include <Model/Synth/SynthConfig.hpp>
#include <Model/Synth/LfoModel.hpp>
#include <Model/Synth/EnvModel.hpp>

namespace Xts {

struct XTS_ALIGN CvModel
{
  LfoModel lfos[XTS_VOICE_LFO_COUNT];
  EnvModel envs[XTS_VOICE_ENV_COUNT];
};
XTS_CHECK_SIZE(CvModel, 312);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_CV_MODEL_HPP