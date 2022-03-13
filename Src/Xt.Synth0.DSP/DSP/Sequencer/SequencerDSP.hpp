#ifndef XTS_DSP_SEQUENCER_DSP_HPP
#define XTS_DSP_SEQUENCER_DSP_HPP

#include <DSP/Synth/SynthDSP.hpp>
#include <Model/Synth/SynthModel.hpp>
#include <Model/Shared/ParamBinding.hpp>
#include <Model/Sequencer/SequencerModel.hpp>
#include <cstdint>

namespace Xts {

constexpr int MaxVoices = 128;
enum class MoveType { None, Next, End };

struct SeqInput
{
  int frames;
  float rate;
  float* buffer;
};

struct SeqOutput
{
  int64_t pos;
  int row, voices;
  bool clip, exhausted;
};

class SequencerDSP
{
  int _row = -1;
  int _voices = 0;
  int64_t _pos = 0;
  double _fill = 0.0;
  bool _endAudio = false;
  bool _endPattern = false;
  SequencerModel const* _model;
  SynthModel const* _synth;
  ParamBinding const* _binding;
private:
  int _keys[MaxVoices];
  int _active[XTS_SEQUENCER_MAX_KEYS];
  SynthDSP _dsps[MaxVoices];
  int64_t _started[MaxVoices];
  SynthModel _synths[MaxVoices];
private:
  void Automate();
  void ApplyActive();
  int Take(int key, int voice);
  void Return(int key, int voice);
  int Take(int key, bool& exhausted);
  bool Trigger(SeqInput const& input);
  MoveType Move(SeqInput const& input);
  FloatSample Next(SeqInput const& input, bool& exhausted);
public:
  bool End() const { return _endAudio; }
  void Render(SeqInput const& input, SeqOutput& output);
  void Init(SequencerModel const* model, SynthModel const* synth, ParamBinding const* binding);
};

} // namespace Xts
#endif // XTS_DSP_SEQUENCER_DSP_HPP