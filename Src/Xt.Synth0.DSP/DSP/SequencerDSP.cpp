#include "SequencerDSP.hpp"
#include <cassert>

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

bool 
SequencerDSP::RowUpdated()
{
	bool result = _previousRow != _currentRow;
	_previousRow = _currentRow;
	return result;
}

void 
SequencerDSP::Next(SequencerModel const& seq, SynthModel& synth, float rate, float* l, float* r)
{
	if (UpdateRow(seq, synth, rate))
		_pattern.Automate(seq.edit, seq.pattern.rows[_currentRow], synth);
	_synth.Next(synth, rate, l, r);
	_streamPosition++;
}

void
SequencerDSP::ProcessBuffer(
	SequencerModel const& seq, SynthModel& synth, float rate,
	float* buffer, int32_t frames, int32_t* currentRow, int64_t* streamPosition)
{
  float l;
  float r;
	for (int f = 0; f < frames; f++)
	{
		Next(seq, synth, rate, &l, &r);
		buffer[f * 2] = l;
		buffer[f * 2 + 1] = r;
	}
	*currentRow = _currentRow;
	*streamPosition = _streamPosition;
}

bool 
SequencerDSP::UpdateRow(SequencerModel const& seq, SynthModel& synth, float rate)
{
	int lpb = seq.edit.lpb;
	int pats = seq.edit.pats;
	int rows = seq.edit.rows;
	int bpm = synth.global.bpm;
  int maxRows = TrackConstants::MaxRows;
	_rowFactor += bpm * lpb / (60.0 * rate);
	if (_rowFactor < 1.0) 
    return RowUpdated();
	_rowFactor = 0.0f;
  _currentRow++;
  if(_currentRow % maxRows == rows)
  {
     _currentRow += maxRows - rows;
     assert(_currentRow % maxRows == 0);
  }
  if(_currentRow == pats * maxRows) _currentRow = 0;
	return RowUpdated();
}

} // namespace Xts