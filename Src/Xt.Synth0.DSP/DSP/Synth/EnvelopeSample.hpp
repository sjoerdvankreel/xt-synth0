#ifndef XTS_DSP_SYNTH_ENVELOPE_SAMPLE_HPP
#define XTS_DSP_SYNTH_ENVELOPE_SAMPLE_HPP

namespace Xts {

enum class EnvelopeStage
{ 
  Delay,
  Attack,
  Hold,
  Decay,
  Sustain,
  Release,
  End
};

struct EnvelopeSample
{ 
  float value;
  bool switchedStage;
  EnvelopeStage stage;
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_ENVELOPE_SAMPLE_HPP