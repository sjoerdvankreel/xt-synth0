#ifndef XTS_SEQ_DSP_HPP
#define XTS_SEQ_DSP_HPP

#include "SynthDSP.hpp"
#include "PatternDSP.hpp"
#include "../Model/SeqModel.hpp"
#include "../Model/SynthModel.hpp"
#include <cstdint>

namespace Xts {

struct SeqOutput
{
  int32_t currentRow;
  int64_t streamPosition;
};

class SeqDSP
{
  SynthDSP _synth;
  PatternDSP const _pattern;

  int _currentRow = 0;
	int _previousRow = -1;
  double _rowFactor = 0.0;
  uint64_t _streamPosition = 0;

  bool RowUpdated();
  bool UpdateRow(SeqModel const& seq, SynthModel& synth, float rate);
  void Next(SeqModel const& seq, SynthModel& synth, float rate, SynthOutput& output);

public:
  void Init();
  void ProcessBuffer(SeqModel const& seq, SynthModel& synth, float rate, float* buffer, int32_t frames, SeqOutput& output);
};

} // namespace Xts
#endif // XTS_SEQ_DSP_HPP