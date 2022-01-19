#ifndef XTS_SEQ_DSP_HPP
#define XTS_SEQ_DSP_HPP

#include "SynthDSP.hpp"
#include "../Model/SeqModel.hpp"
#include "../Model/SynthModel.hpp"
#include <cstdint>

namespace Xts {

inline const int MaxVoices = 128;

struct SeqInput
{
  int frames;
  float rate;
  int64_t pos;
  float* buffer;
  SynthModel* synth;
public:
  SeqInput() = default;
  SeqInput(SeqInput const&) = delete;
};

struct SeqOutput
{
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
  double _fill = 0.0;
  int _keys[MaxVoices];
  SynthDSP _dsps[MaxVoices];
  int64_t _started[MaxVoices];
  SynthModel _models[MaxVoices];
public:
  SeqDSP() = default;
  SeqDSP(SeqDSP const&) = delete;
public:
  void Init();
  void Render(SeqInput const& input, SeqOutput& output);
private:
  bool Moved(int row);
  void Return(int key, int voice);
  bool Move(SeqInput const& input);
  int Take(int key, int64_t pos, SeqOutput& output);
  AudioOutput Next(SeqInput const& input, SeqOutput& output);
  void Automate(PatternFx const& fx, SynthModel& synth) const;
  void Automate(EditModel const& edit, PatternRow const& row, SynthModel& synth) const;
};

} // namespace Xts
#endif // XTS_SEQ_DSP_HPP