#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/Utility.hpp>
#include <DSP/Synth/SynthDSP.hpp>
#include <DSP/Sequencer/SequencerDSP.hpp>
#include <Model/Shared/ParamInfo.hpp>
#include <Model/Shared/AutomationAction.hpp>

#include <cassert>
#include <algorithm>

namespace Xts {

SequencerDSP::
SequencerDSP(float rate, size_t frames) :
SequencerDSP()
{
  _rate = rate;
  _fill = 0.0;
  _endPattern = false;
  _output.row = -1;
  _output.voices = 0;
  _output.position = 0;
  _output.end = XtsFalse;
  _output.clip = XtsFalse;
  _output.exhausted = XtsFalse;
  _buffer.resize(frames * 2);
}

SequencerOutput const*
SequencerDSP::Render(int32_t frames, AutomationAction const* actions, int count)
{
  bool clipped = false;
  _output.end = XtsFalse;
  _output.clip = XtsFalse;
  _output.exhausted = XtsFalse;
  for(int i = 0; i < count; i++)
    _synth->Automate(actions[i].target, actions[i].value);
  for (int f = 0; f < frames; f++)
  {
    auto out = Next();
    _buffer[f * 2] = Clip(out.left, clipped);
    _buffer[f * 2 + 1] = Clip(out.right, clipped);
    _output.position++;
  }
  _output.clip = clipped;
  _output.buffer = _buffer.data();
  return &_output;
}

FloatSample
SequencerDSP::Next()
{
  SequencerMove move = Move();
  if (move == SequencerMove::Next)
    _output.exhausted |= (Trigger() ? XtsTrue : XtsFalse);
  else if (move == SequencerMove::End)
    _synth->ReleaseAll();
  auto result = _synth->Next();
  _output.voices = _synth->Voices();
  _output.end = _synth->Voices() == 0 && _endPattern;
  return result;
}

void
SequencerDSP::Automate()
{
  for (int f = 0; f < _model.edit.fxs; f++)
  {
    auto const& fx = _model.pattern.rows[_output.row].fx[f];
    if(fx.target > 0) _synth->Automate(fx.target - 1, fx.value);
  }
}

bool
SequencerDSP::Trigger()
{
  bool result = false;
  for (int k = 0; k < _model.edit.keys; k++)
  {
    auto const& key = _model.pattern.rows[_output.row].keys[k];
    if (key.note >= PatternNote::Off)
      _synth->Release(k);
    if (key.note < PatternNote::C) continue;
    float velocity = Param::Level(key.velocity);
    UnitNote note = static_cast<UnitNote>(static_cast<int>(key.note) - 2);
    result |= _synth->Trigger(k, key.octave, note, velocity, _output.position);
  }
  Automate();
  return result;
}

SequencerMove
SequencerDSP::Move()
{
  int current = _output.row;
  auto const& edit = _model.edit;
  if (_endPattern) return SequencerMove::None;
  if (current == -1) return _output.row = 0, SequencerMove::Next;
  _fill += edit.bpm * edit.lpb / (60.0 * _rate);
  if (_fill < 1.0) return SequencerMove::None;
  _fill = 0.0f;
  _output.row++;
  if (_output.row % XTS_SEQUENCER_MAX_ROWS == edit.rows) _output.row += XTS_SEQUENCER_MAX_ROWS - edit.rows;
  if (_output.row != edit.patterns * XTS_SEQUENCER_MAX_ROWS) return SequencerMove::Next;
  if (edit.loop) return _output.row = 0, SequencerMove::Next;
  _endPattern = true;
  _output.row = current;
  return SequencerMove::End;
}

} // namespace Xts