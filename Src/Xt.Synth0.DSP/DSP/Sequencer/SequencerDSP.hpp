#ifndef XTS_DSP_SEQUENCER_DSP_HPP
#define XTS_DSP_SEQUENCER_DSP_HPP

#include <DSP/Synth/SynthDSP.hpp>
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
  SynthDSP _synth;
  double _fill = 0.0;
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
  SequencerOutput const* Render(int32_t frames);
public:
  SequencerDSP() = default;
  SequencerDSP(SequencerModel const* model, struct SynthModel const* synth, struct ParamBinding const* binding, float rate, size_t frames);
};

} // namespace Xts
#endif // XTS_DSP_SEQUENCER_DSP_HPP