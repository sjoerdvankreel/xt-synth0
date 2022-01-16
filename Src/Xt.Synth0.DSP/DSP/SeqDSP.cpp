#include "SeqDSP.hpp"
#include "DSP.hpp"
#include <cstring>
#include <cassert>

namespace Xts {

void 
SeqDSP::ReleaseVoice(int key, int voice)
{ 
  _voicesUsed--;
  _keysToVoices[key] = -1;
  _voicesStarted[voice] = -1;
}

bool
SeqDSP::RowUpdated(int currentRow)
{
	bool result = _previousRow != currentRow;
	_previousRow = currentRow;
	return result;
}

void
SeqDSP::Init(SeqState& state)
{
  _voicesUsed = 0;
  _rowFactor = 0.0;
  _previousRow = -1;
  for (int i = 0; i < MaxVoices; i++)
    _voicesStarted[i] = -1;
  for (int i = 0; i < TrackConstants::MaxKeys; i++)
    _keysToVoices[i] = -1;
  memset(&state, 0, sizeof(state));
}

int
SeqDSP::TakeVoice(int key, int64_t pos)
{
  int victim = -1;
  int64_t victimStart = 0x7FFFFFFFFFFFFFFF;
  for(int i = 0; i < MaxVoices; i++)
  {
    if(_voicesStarted[i] == -1)
    {
      _voicesUsed++;
      _keysToVoices[key] = i;
      _voicesStarted[i] = pos;
      return i;
    }
    if(_voicesStarted[i] < victimStart)
    {
			victim = i;
			victimStart = _voicesStarted[i];
    }
  }
  assert(0 <= victim && victim < MaxVoices);
  _keysToVoices[key] = victim;
  _voicesStarted[victim] = pos;
  return victim;
}

void
SeqDSP::ProcessBuffer(SeqState& state)
{
	bool clip;
	SeqOutput output;
	state.clip = false;
	for (int f = 0; f < state.frames; f++)
	{
		Next(state, output);
		state.buffer[f * 2] = Clip(output.l, clip);
		state.clip |= clip? XtsTrue: XtsFalse;
		state.buffer[f * 2 + 1] = Clip(output.r, clip);
		state.clip |= clip ? XtsTrue : XtsFalse;
	}
  state.voices = _voicesUsed;
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
SeqDSP::Next(SeqState& state, SeqOutput& output)
{
  SynthOutput sout;
  memset(&output, 0, sizeof(output));
	bool updated = UpdateRow(state);
  auto row = state.seq->pattern.rows[state.currentRow];
	if (updated) _pattern.Automate(state.seq->edit, row, *state.synth);
  for(int k = 0; k < state.seq->edit.keys; k++)
  {
    auto key = row.keys[k];
    auto note = static_cast<PatternNote>(key.note);
    auto unitNote = static_cast<UnitNote>(static_cast<int>(note) - 2);
    if(updated && note >= PatternNote::C)
    {
      int voice = TakeVoice(k, state.streamPosition);
      _voices[voice].Init(key.oct, unitNote);
    }
    if(updated && note == PatternNote::Off)
      _voices[_keysToVoices[k]].Release();
    if(_keysToVoices[k] != -1)
    {
      _voices[_keysToVoices[k]].Next(*state.synth, state.rate, sout);
      output.l += sout.l;
      output.r += sout.r;
      if(sout.end) ReleaseVoice(k, _keysToVoices[k]);
    }
  }
	state.streamPosition++;
}

} // namespace Xts