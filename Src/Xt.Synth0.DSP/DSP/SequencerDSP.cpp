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

void
SequencerDSP::ProcessBuffer(
	SequencerModel const& seq, SynthModel& synth, float rate,
	float* buffer, int32_t frames, int32_t* currentRow, int64_t* streamPosition)
{
	for (int f = 0; f < frames; f++)
	{
		float sample = Next(seq, synth, rate);
		buffer[f * 2] = sample;
		buffer[f * 2 + 1] = sample;
	}
	*currentRow = _currentRow;
	*streamPosition = _streamPosition;
}

bool 
SequencerDSP::UpdateRow(SequencerModel const& seq, SynthModel& synth, float rate)
{
	int bpm = synth.global.bpm;
	int pats = seq.edit.pats;
	int rowsPerPattern = TrackConstants::PatternRows;
	int totalRows = pats * rowsPerPattern;
	_rowFactor += bpm * TrackConstants::BeatRows / (60.0 * rate);
	if (_rowFactor < 1.0) return RowUpdated();
	_rowFactor = 0.0f;
	if (_currentRow < totalRows - 1) _currentRow++;
	else _currentRow = 0;
	return RowUpdated();
}

} // namespace Xts