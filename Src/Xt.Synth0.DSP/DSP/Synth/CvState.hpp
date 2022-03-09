#ifndef XTS_DSP_SYNTH_CV_STATE_HPP
#define XTS_DSP_SYNTH_CV_STATE_HPP

#include <DSP/CvSample.hpp>
#include <DSP/EnvSample.hpp>
#include <Model/Synth/Config.hpp>

namespace Xts {

struct CvState
{ 
  float velocity;
  CvSample lfos[XTS_SYNTH_LFO_COUNT];
  EnvSample envs[XTS_SYNTH_ENV_COUNT];
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_CV_STATE_HPP