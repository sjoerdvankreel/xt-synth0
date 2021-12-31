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

bool 
SequencerDSP::RowUpdated()
{
	bool result = _previousRow != _currentRow;
	_previousRow = _currentRow;
	return result;
}

float 
SequencerDSP::Next(SequencerModel const& seq, SynthModel& synth, float rate)
{
	if (UpdateRow(seq, synth, rate))
		_pattern.Automate(seq.edit, seq.pattern.rows[_currentRow], synth);
	float result = _synth.Next(synth, rate);
	_streamPosition++;
  return result;
}

bool 
SequencerDSP::UpdateRow(SequencerModel const& seq, SynthModel& synth, float rate)
{
	int bpm = synth.global.bpm;
	int patterns = seq.edit.pats;
	int rowsPerPattern = PatternModel::PatternRows;
	int totalRows = patterns * rowsPerPattern;
	_rowFactor += bpm * PatternModel::BeatRows / (60.0 * rate);
	if (_rowFactor < 1.0) return RowUpdated();
	_rowFactor = 0.0f;
	if (_currentRow < totalRows - 1) _currentRow++;
	else _currentRow = 0;
	return RowUpdated();
}

} // namespace Xts