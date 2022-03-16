#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/Utility.hpp>
#include <DSP/Sequencer/SequencerDSP.hpp>
#include <Model/Shared/ParamInfo.hpp>

#include <cassert>
#include <algorithm>

namespace Xts {

FloatSample
SequencerDSP::Next(float rate)
{
  FloatSample result = { 0 };
  SequencerMove move = Move(rate);  
  if (move == SequencerMove::Next)
    _output.exhausted |= Trigger(rate);
  else if(move == SequencerMove::End)
    for(int k = 0; k < XTS_SEQUENCER_MAX_KEYS; k++)
      if (_active[k] != -1) _dsps[_active[k]].Release();  
  for (int v = 0; v < XTS_SEQUENCER_MAX_VOICES; v++)
  {
    if (_keys[v] == -1) continue;
    result += _dsps[v].Next();
    if (_dsps[v].End()) Return(_keys[v], v);
  }
  return result;
}

SequencerOutput const*
SequencerDSP::Render(int32_t frames, float rate)
{
  bool clipped = false;
  _output.end = XtsFalse;
  _output.clip = XtsFalse;
  _output.exhausted = XtsFalse;
  for (int f = 0; f < frames; f++)
  {
    auto out = Next(rate);
    _buffer[f * 2] = Clip(out.left, clipped);
    _buffer[f * 2 + 1] = Clip(out.right, clipped);
    _output.position++;
  }
  _output.clip = clipped;
  _output.buffer = _buffer.data();
  return &_output;
}

void
SequencerDSP::Automate()
{
  for (int f = 0; f < _model->edit.fxs; f++)
  {
    auto const& fx = _model->pattern.rows[_output.row].fx[f];
    assert(0 <= fx.value && fx.value < 256);
    assert(0 <= fx.target && fx.target < 256);
    if (fx.target == 0 || fx.target > XTS_SYNTH_PARAM_COUNT) return;
    int32_t* param = _binding->params[fx.target - 1];
    ParamInfo const& info = SynthModel::Params()[fx.target - 1];
    *param = std::clamp(fx.value, info.min, info.max);
  }
}

void
SequencerDSP::Return(int key, int voice)
{
  _output.voices--;
  _keys[voice] = -1;
  _started[voice] = -1;
  _output.end = _output.voices == 0 && _endPattern;
  assert(0 <= key && key < XTS_SEQUENCER_MAX_KEYS);
  assert(0 <= voice && voice < XTS_SEQUENCER_MAX_VOICES);
  assert(0 <= _output.voices && _output.voices < XTS_SEQUENCER_MAX_VOICES);
}

int
SequencerDSP::Take(int key, int voice)
{
  _output.voices++;
  _keys[voice] = key;
  _active[key] = voice;
  _started[voice] = _output.position;
  assert(0 <= _output.voices && _output.voices <= XTS_SEQUENCER_MAX_VOICES);
  return voice;
}

int
SequencerDSP::Take(int key)
{
  int victim = -1;
  assert(_output.position >= 0);
  assert(0 <= key && key < XTS_SEQUENCER_MAX_KEYS);
  int64_t victimStart = 0x7FFFFFFFFFFFFFFF;
  for (int i = 0; i < XTS_SEQUENCER_MAX_VOICES; i++)
  {
    if (_started[i] == -1) return Take(key, i);
    if (_started[i] < victimStart) victimStart = _started[victim = i];
  }
  _keys[victim] = key;
  _active[key] = victim;
  _output.exhausted = XtsTrue;
  _started[victim] = _output.position;
  assert(0 <= victim && victim < XTS_SEQUENCER_MAX_VOICES);
  assert(0 <= _output.voices && _output.voices <= XTS_SEQUENCER_MAX_VOICES);
  return victim;
}

bool
SequencerDSP::Trigger(float rate)
{
  Automate();
  bool result = false;
  bool exhausted = false;
  for (int k = 0; k < _model->edit.keys; k++)
  {
    auto const& key = _model->pattern.rows[_output.row].keys[k];
    if (key.note >= PatternNote::Off)
      for (int v = 0; v < XTS_SEQUENCER_MAX_VOICES; v++)
        if (_keys[v] == k) _dsps[v].Release();
    if (key.note < PatternNote::C) continue;
    int voice = Take(k, exhausted);
    result |= exhausted;
    _synths[voice] = *_synth;
    float velocity = Param::Level(key.velocity);
    float bpm = static_cast<float>(_model->edit.bpm);
    UnitNote note = static_cast<UnitNote>(static_cast<int>(key.note) - 2);
    new (&_dsps[voice]) SynthDSP(&_synths[voice], key.octave, note, velocity, bpm, rate);
  }
  return result;
}

SequencerMove
SequencerDSP::Move(float rate)
{
  int current = _output.row;
  auto const& edit = _model->edit;
  if (_endPattern) return SequencerMove::None;
  if (current == -1) return _output.row = 0, SequencerMove::Next;
  _fill += edit.bpm * edit.lpb / (60.0 * rate);
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

SequencerDSP::
SequencerDSP(SequencerModel const* model, SynthModel const* synth, ParamBinding const* binding, size_t frames):
SequencerDSP()
{
  _fill = 0.0;
  _model = model;
  _synth = synth;
  _binding = binding;
  _endPattern = false;
  _output.row = -1;
  _output.voices = 0;
  _output.position = 0;
  _output.end = XtsFalse;
  _output.clip = XtsFalse;
  _output.exhausted = XtsFalse;
  _buffer.resize(frames * 2);
  for (int i = 0; i < XTS_SEQUENCER_MAX_KEYS; i++) _active[i] = -1;
  for (int i = 0; i < XTS_SEQUENCER_MAX_VOICES; i++) _started[i] = _keys[i] = -1;
}

} // namespace Xts