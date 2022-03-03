#ifndef XTS_DSP_SYNTH_CV_SAMPLE_HPP
#define XTS_DSP_SYNTH_CV_SAMPLE_HPP

#include <DSP/Utility.hpp>

namespace Xts {

struct CvSample
{ 
  float value;
  bool bipolar;
  CvSample Sanity() const;
};

inline CvSample
CvSample::Sanity() const
{
  if(bipolar) BipolarSanity(value);
  else UnipolarSanity(value);
  return *this;
}

} // namespace Xts
#endif // XTS_DSP_SYNTH_CV_SAMPLE_HPP