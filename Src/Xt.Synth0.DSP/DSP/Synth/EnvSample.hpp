#ifndef XTS_DSP_SYNTH_ENV_SAMPLE_HPP
#define XTS_DSP_SYNTH_ENV_SAMPLE_HPP

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
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_ENV_SAMPLE_HPP