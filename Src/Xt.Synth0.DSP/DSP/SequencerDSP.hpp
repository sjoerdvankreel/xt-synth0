#ifndef XTS_SEQUENCER_DSP_HPP
#define XTS_SEQUENCER_DSP_HPP

#include "SynthDSP.hpp"
#include "PatternDSP.hpp"
#include "../Model/SynthModel.hpp"
#include "../Model/SequencerModel.hpp"
#include <cstdint>

namespace Xts {

class SequencerDSP
{
  int _currentRow = 0;
	int _previousRow = -1;
  double _rowFactor = 0.0;
  uint64_t _streamPosition = 0;

  SynthDSP _synth;
  PatternDSP const _pattern;

  bool RowUpdated();
  bool UpdateRow(SequencerModel const& seq, SynthModel& synth, float rate);

public:
  void Reset();
  int CurrentRow() const { return _currentRow; }
  uint64_t StreamPosition() const { return _streamPosition; }
  float Next(SequencerModel const& seq, SynthModel& synth, float rate);
};

} // namespace Xts
#endif // XTS_SEQUENCER_DSP_HPP