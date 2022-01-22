#ifndef XTS_SEQ_DSP_HPP
#define XTS_SEQ_DSP_HPP

#include "SynthDSP.hpp"
#include "../Model/SeqModel.hpp"
#include "../Model/SynthModel.hpp"
#include <cstdint>

namespace Xts {

constexpr int MaxVoices = 128;

struct SeqInput
{
  int frames;
  float rate;
  float* buffer;
public:
  SeqInput() = default;
  SeqInput(SeqInput const&) = delete;
};

struct SeqOutput
{
  int64_t pos;
  int row, voices;
  bool clip, exhausted;
public:
  SeqOutput() = default;
  SeqOutput(SeqOutput const&) = delete;
};

class SeqDSP
{
  int _row = -1;
  int _voices = 0;
  int64_t _pos = 0;
  double _fill = 0.0;
  SeqModel const* _model;
  SynthModel const* _synth;
private:
  int _keys[MaxVoices];
  SynthDSP _dsps[MaxVoices];
  int64_t _started[MaxVoices];
  SynthInput _inputs[MaxVoices];
  SynthModel _synths[MaxVoices];
private:
  void Automate() const;
  int Take(int key, int voice);
  void Return(int key, int voice);
  bool Move(SeqInput const& input);
  int Take(int key, bool& exhausted);
  void Trigger(SeqInput const& input, bool& exhausted);
  AudioOutput Next(SeqInput const& input, bool& exhausted);
public:
  void Render(SeqInput const& input, SeqOutput& output);
  void Init(SeqModel const* model, SynthModel const* synth);
};

} // namespace Xts
#endif // XTS_SEQ_DSP_HPP