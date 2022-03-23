#ifndef XTS_DSP_SEQUENCER_DSP_HPP
#define XTS_DSP_SEQUENCER_DSP_HPP

#include <DSP/Shared/AudioSample.hpp>
#include <Model/Sequencer/SequencerModel.hpp>
#include <cstdint>

namespace Xts {

struct XTS_ALIGN SequencerOutput
{
  int32_t row;
  int32_t voices;
  XtsBool end;
  XtsBool clip;
  XtsBool exhausted;
  int32_t pad__;
  float* buffer;
  int64_t position;
};
XTS_CHECK_SIZE(SequencerOutput, 40);

enum class SequencerMove { None, Next, End };

class SequencerDSP
{
  float _rate;
  double _fill = 0.0;
  class SynthDSP* _synth;
  SequencerOutput _output;
  bool _endPattern = false;
  std::vector<float> _buffer;
  SequencerModel const* _model;
private:
  bool Trigger();
  void Automate();
  FloatSample Next();
  SequencerMove Move();
public:
  SequencerDSP() = default;
  SequencerDSP(SequencerModel const* model, float rate, size_t frames);
public:
  void Connect(class SynthDSP* synth) { _synth = synth; }
  SequencerOutput const* Render(int32_t frames, struct AutomationAction const* actions, int count);
};

} // namespace Xts
#endif // XTS_DSP_SEQUENCER_DSP_HPP