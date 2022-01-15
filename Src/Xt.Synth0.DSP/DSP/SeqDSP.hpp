#ifndef XTS_SEQ_DSP_HPP
#define XTS_SEQ_DSP_HPP

#include "SynthDSP.hpp"
#include "PatternDSP.hpp"
#include "../Model/SeqModel.hpp"
#include "../Model/SynthModel.hpp"
#include <cstdint>

namespace Xts {

struct XTS_ALIGN SeqState
{
  float rate;
  int32_t frames;
  int32_t currentRow;
  int32_t pad__;
  int64_t streamPosition;
  float* buffer;
  SynthModel* synth;
  SeqModel const* seq;
  SeqState() = default;
};

class SeqDSP
{
  int _previousRow = -1;
  double _rowFactor = 0.0;
  SynthDSP _synth;
  PatternDSP const _pattern;

public:
  void Init(SeqState& state);
  void ProcessBuffer(SeqState& state);

private:
  bool RowUpdated(int currentRow);
  bool UpdateRow(SeqState& state);
  void Next(SeqState& state, SynthOutput& output);
};

} // namespace Xts
#endif // XTS_SEQ_DSP_HPP