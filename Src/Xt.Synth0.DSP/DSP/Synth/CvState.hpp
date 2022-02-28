#ifndef XTS_DSP_SYNTH_CV_STATE_HPP
#define XTS_DSP_SYNTH_CV_STATE_HPP

#include <Model/Model.hpp>
#include <DSP/Synth/CvSample.hpp>
#include <DSP/Synth/EnvelopeSample.hpp>

namespace Xts {

struct CvState
{ 
  float velocity;
  CvSample lfos[LfoCount];
  EnvelopeSample envelopes[EnvelopeCount];
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_CV_STATE_HPP