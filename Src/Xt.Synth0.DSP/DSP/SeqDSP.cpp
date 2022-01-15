#include "SeqDSP.hpp"
#include <cassert>

namespace Xts {

void 
SeqDSP::Init()
{
  _rowFactor = 0.0;
  _previousRow = -1;
}

void
SeqDSP::ProcessBuffer(SeqState& state)
{
  SynthOutput audio;
	for (int f = 0; f < state.frames; f++)
	{
		Next(state, audio);
		state.buffer[f * 2] = audio.l;
		state.buffer[f * 2 + 1] = audio.r;
	}
}

bool
SeqDSP::RowUpdated(int currentRow)
{
	bool result = _previousRow != currentRow;
	_previousRow = currentRow;
	return result;
}

bool 
SeqDSP::UpdateRow(SeqState&  state)
{
	int lpb = state.seq->edit.lpb;
	int pats = state.seq->edit.pats;
	int rows = state.seq->edit.rows;
	int bpm = state.synth->global.bpm;
  int maxRows = TrackConstants::MaxRows;
	_rowFactor += bpm * lpb / (60.0 * state.rate);
	if (_rowFactor < 1.0) return RowUpdated(state.currentRow);
	_rowFactor = 0.0f;
	state.currentRow++;
  if(state.currentRow % maxRows == rows)
  {
		state.currentRow += maxRows - rows;
    assert(state.currentRow % maxRows == 0);
  }
  if(state.currentRow == pats * maxRows) state.currentRow = 0;
	return RowUpdated(state.currentRow);
}

void
SeqDSP::Next(SeqState& state, SynthOutput& output)
{
	bool updated = UpdateRow(state);
  auto row = state.seq->pattern.rows[state.currentRow];
	if (updated) _pattern.Automate(state.seq->edit, row, *state.synth);
  auto key = row.keys[0];
  auto note = static_cast<PatternNote>(key.note);
  auto unitNote = static_cast<UnitNote>(static_cast<int>(note) - 2);
	if (updated && note == PatternNote::Off) _synth.Release();
	if(updated && note >= PatternNote::C) _synth.Init(key.oct, unitNote);
	_synth.Next(*state.synth, state.rate, output);
	state.streamPosition++;
}

} // namespace Xts