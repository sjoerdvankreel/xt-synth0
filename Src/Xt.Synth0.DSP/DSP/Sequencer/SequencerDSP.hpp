#ifndef XTS_DSP_SEQUENCER_DSP_HPP
#define XTS_DSP_SEQUENCER_DSP_HPP

#include <DSP/Synth/SynthDSP.hpp>
#include <Model/Synth/SynthModel.hpp>
#include <Model/Shared/ParamBinding.hpp>
#include <Model/Shared/SharedConfig.hpp>
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
  double _fill = 0.0;
  SequencerOutput _output;
  bool _endPattern = false;
  SynthModel const* _synth;
  std::vector<float> _buffer;
  ParamBinding const* _binding;
  SequencerModel const* _model;
private:
  void Automate();
  int Take(int key);
  bool Trigger(float rate);
  int Take(int key, int voice);
  FloatSample Next(float rate);
  SequencerMove Move(float rate);
  void Return(int key, int voice);
private:
  int _keys[XTS_SYNTH_MAX_VOICES];
  int _active[XTS_SHARED_MAX_KEYS];
  SynthDSP _dsps[XTS_SYNTH_MAX_VOICES];
  int64_t _started[XTS_SYNTH_MAX_VOICES];
  SynthModel _synths[XTS_SYNTH_MAX_VOICES];
public:
  SequencerOutput const* Render(int32_t frames, float rate);
public:
  SequencerDSP() = default;
  SequencerDSP(SequencerModel const* model, SynthModel const* synth, ParamBinding const* binding, size_t frames);
};

} // namespace Xts
#endif // XTS_DSP_SEQUENCER_DSP_HPP