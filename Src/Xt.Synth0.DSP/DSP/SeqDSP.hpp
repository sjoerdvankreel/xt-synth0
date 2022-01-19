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
  SynthModel* synth;
  SeqModel const* seq;
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
  int _keys[MaxVoices];
  SynthDSP _dsps[MaxVoices];
  int64_t _started[MaxVoices];
  AudioInput _inputs[MaxVoices]; 
  SynthModel _models[MaxVoices];
public:
  SeqDSP() = default;
  SeqDSP(SeqDSP const&) = delete;
public:
  void Init();
  void Render(SeqInput const& input, SeqOutput& output);
private:
  int Take(int key, int voice);
  void Return(int key, int voice);
  bool Move(SeqInput const& input);
  int Take(int key, bool& exhausted);
  void Automate(SeqModel const& seq) const;
  void Trigger(SeqInput const& input, bool& exhausted);
  AudioOutput Next(SeqInput const& input, bool& exhausted);
};

} // namespace Xts
#endif // XTS_SEQ_DSP_HPP