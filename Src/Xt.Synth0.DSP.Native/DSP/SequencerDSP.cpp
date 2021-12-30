#include "SequencerDSP.hpp"

namespace Xts {

void 
SequencerDSP::Reset()
{
  _currentRow = 0;
  _rowFactor = 0.0;
  _previousRow = -1;
  _streamPosition = 0;
  _synth.Reset();
}

float
SequencerDSP::Next(SequencerModel const& seq, SynthModel& synth, float rate)
{
  _streamPosition++;
  return 0.0f;
}

} // namespace Xts