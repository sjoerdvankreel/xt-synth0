#ifndef XTS_SEQ_DSP_HPP
#define XTS_SEQ_DSP_HPP

#include "SynthDSP.hpp"
#include "PatternDSP.hpp"
#include "../Model/SeqModel.hpp"
#include "../Model/SynthModel.hpp"
#include <cstdint>

namespace Xts {

struct SeqState
{
  float rate;
  float* buffer;
  int32_t frames;
  SynthModel* synth;
  int32_t currentRow;
  SeqModel const* seq;
  int64_t streamPosition;
  SeqState() = default;
};

class SeqDSP
{
  int _previousRow = -1;
  double _rowFactor = 0.0;
  SynthDSP _synth;
  PatternDSP const _pattern;

public:
  void Init();
  void ProcessBuffer(SeqState& state);

private:
  bool RowUpdated(int currentRow);
  bool UpdateRow(SeqState& state);
  void Next(SeqState& state, SynthOutput& output);
};

} // namespace Xts
#endif // XTS_SEQ_DSP_HPP