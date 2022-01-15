#ifndef XTS_SEQUENCER_DSP_HPP
#define XTS_SEQUENCER_DSP_HPP

#include "SynthDSP.hpp"
#include "PatternDSP.hpp"
#include "../Model/SynthModel.hpp"
#include "../Model/SequencerModel.hpp"
#include <cstdint>

namespace Xts {

struct SequencerOutput
{
  int32_t currentRow;
  int64_t streamPosition;
};

class SequencerDSP
{
  SynthDSP _synth;
  PatternDSP const _pattern;

  int _currentRow = 0;
	int _previousRow = -1;
  double _rowFactor = 0.0;
  uint64_t _streamPosition = 0;

  bool RowUpdated();
  bool UpdateRow(SequencerModel const& seq, SynthModel& synth, float rate);
  void Next(SequencerModel const& seq, SynthModel& synth, float rate, SynthOutput& output);

public:
  void Init();
  void ProcessBuffer(SequencerModel const& seq, SynthModel& synth, float rate, float* buffer, int32_t frames, SequencerOutput& output);
};

} // namespace Xts
#endif // XTS_SEQUENCER_DSP_HPP