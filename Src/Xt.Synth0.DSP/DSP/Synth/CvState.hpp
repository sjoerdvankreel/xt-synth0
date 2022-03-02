#ifndef XTS_DSP_SYNTH_CV_STATE_HPP
#define XTS_DSP_SYNTH_CV_STATE_HPP

#include <Model/Synth/Config.hpp>
#include <DSP/Synth/CvSample.hpp>
#include <DSP/Synth/EnvelopeSample.hpp>

namespace Xts {

struct CvState
{ 
  float velocity;
  CvSample lfos[XTS_SYNTH_LFO_COUNT];
  EnvelopeSample envelopes[XTS_SYNTH_ENV_COUNT];
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_CV_STATE_HPP