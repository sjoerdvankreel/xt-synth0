#ifndef XTS_SEQUENCER_DSP_HPP
#define XTS_SEQUENCER_DSP_HPP

#include "SynthDSP.hpp"
#include "PatternDSP.hpp"
#include "../Model/SynthModel.hpp"
#include "../Model/SequencerModel.hpp"
#include <vector>

namespace Xts {

class SequencerDSP
{
  int _currentRow = 0;
	int _previousRow = -1;
  double _rowFactor = 0.0;
  long _streamPosition = 0;

  SynthDSP _synth;
  PatternDSP const _pattern;

public:
  void Reset();
  int CurrentRow() const { return _currentRow; }
  float Next(SequencerModel const& seq, SynthModel& synth, float rate);
  SequencerDSP(std::vector<int*> params) : _synth(params), _pattern() {}
};

} // namespace Xts
#endif // XTS_SEQUENCER_DSP_HPP