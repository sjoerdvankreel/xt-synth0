#ifndef XTS_DSP_SYNTH_ENV_SAMPLE_HPP
#define XTS_DSP_SYNTH_ENV_SAMPLE_HPP

#include <DSP/Utility.hpp>

namespace Xts {

enum class EnvStage
{ 
  Delay,
  Attack,
  Hold,
  Decay,
  Sustain,
  Release,
  End
};

struct EnvSample
{ 
  float value;
  EnvStage stage;
  bool switchedStage;
  EnvSample Sanity() const;
};

inline EnvSample
EnvSample::Sanity() const
{
  UnipolarSanity(value);
  return *this;
}

} // namespace Xts
#endif // XTS_DSP_SYNTH_ENV_SAMPLE_HPP