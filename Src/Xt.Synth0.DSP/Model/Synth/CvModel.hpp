#ifndef XTS_MODEL_SYNTH_CV_MODEL_HPP
#define XTS_MODEL_SYNTH_CV_MODEL_HPP

#include <Model/Model.hpp>
#include <Model/Synth/LfoModel.hpp>
#include <Model/Synth/EnvelopeModel.hpp>

namespace Xts {

struct XTS_ALIGN CvModel
{
  LfoModel lfos[LfoCount];
  EnvModel envs[EnvelopeCount];
};
XTS_CHECK_SIZE(CvModel, 288);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_CV_MODEL_HPP