#ifndef XTS_MODEL_SYNTH_CV_MODEL_HPP
#define XTS_MODEL_SYNTH_CV_MODEL_HPP

#include <Model/Synth/Config.hpp>
#include <Model/Synth/LfoModel.hpp>
#include <Model/Synth/EnvModel.hpp>

namespace Xts {

struct XTS_ALIGN CvModel
{
  LfoModel lfos[XTS_SYNTH_LFO_COUNT];
  EnvModel envs[XTS_SYNTH_ENV_COUNT];
};
XTS_CHECK_SIZE(CvModel, 288);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_CV_MODEL_HPP