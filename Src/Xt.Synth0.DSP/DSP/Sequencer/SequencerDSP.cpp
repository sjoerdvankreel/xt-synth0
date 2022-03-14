#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/Utility.hpp>
#include <DSP/Sequencer/SequencerDSP.hpp>
#include <Model/Shared/ParamInfo.hpp>

#include <cassert>
#include <algorithm>

namespace Xts {

void
SequencerDSP::Return(int key, int voice)
{
  _voices--;
  _keys[voice] = -1;
  _started[voice] = -1;
  _endAudio = _voices == 0 && _endPattern;
  assert(0 <= key && key < XTS_SEQUENCER_MAX_KEYS);
  assert(0 <= voice && voice < XTS_SEQUENCER_MAX_VOICES);
  assert(0 <= _voices && _voices < XTS_SEQUENCER_MAX_VOICES);
}

int
SequencerDSP::Take(int key, int voice)
{
  _voices++;
  _keys[voice] = key;
  _active[key] = voice;
  _started[voice] = _position;
  assert(0 <= _voices && _voices <= XTS_SEQUENCER_MAX_VOICES);
  return voice;
}

void
SequencerDSP::Automate()
{
  for (int f = 0; f < _model->edit.fxs; f++)
  {
    auto const& fx = _model->pattern.rows[_row].fx[f];
    assert(0 <= fx.value && fx.value < 256);
    assert(0 <= fx.target && fx.target < 256);
    if (fx.target == 0 || fx.target > XTS_SYNTH_PARAM_COUNT) return;
    int32_t* param = _binding->params[fx.target - 1];
    ParamInfo const& info = SynthModel::Params()[fx.target - 1];
    *param = std::clamp(fx.value, info.min, info.max);
  }
}

FloatSample
SequencerDSP::Next(SequencerInput const& input, bool& exhausted)
{
  exhausted = false;
  FloatSample result = { 0 };
  SequencerMove move = Move(input);
  
  if (move == SequencerMove::Next)
    Automate(), exhausted = Trigger(input);
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

void
SequencerDSP::Render(SequencerInput const& input, SequencerOutput& output)
{
  bool exhausted;
  output.clip = false;
  output.exhausted = false;
  for (int f = 0; f < input.frames; f++)
  {
    auto out = Next(input, exhausted);
    output.exhausted |= exhausted;
    input.buffer[f * 2] = Clip(out.left, output.clip);
    input.buffer[f * 2 + 1] = Clip(out.right, output.clip);
    _position++;
  }
  output.row = _row;
  output.end = _endAudio;
  output.voices = _voices;
  output.position = _position;
}

SequencerMove
SequencerDSP::Move(SequencerInput const& input)
{
  int current = _row;
  auto const& edit = _model->edit;
  if (_endPattern) return SequencerMove::None;
  if (_row == -1) return _row = 0, SequencerMove::Next;
  _fill += edit.bpm * edit.lpb / (60.0 * input.rate);
  if (_fill < 1.0) return SequencerMove::None;
  _row++;
  _fill = 0.0f;
  if (_row % XTS_SEQUENCER_MAX_ROWS == edit.rows) _row += XTS_SEQUENCER_MAX_ROWS - edit.rows;
  if (_row != edit.patterns * XTS_SEQUENCER_MAX_ROWS) return SequencerMove::Next;
  if (edit.loop) return _row = 0, SequencerMove::Next;
  _row = current;
  _endPattern = true;
  return SequencerMove::End;
}

void
SequencerDSP::Init(SequencerModel const* model, SynthModel const* synth, ParamBinding const* binding)
{
  _row = -1;
  _fill = 0.0;
  _voices = 0;
  _position = 0;
  _model = model;
  _synth = synth;
  _binding = binding;
  _endAudio = false;
  _endPattern = false;
  for(int i = 0; i < XTS_SEQUENCER_MAX_KEYS; i++) _active[i] = -1;
  for (int i = 0; i < XTS_SEQUENCER_MAX_VOICES; i++) _started[i] = _keys[i] = -1;
}

int
SequencerDSP::Take(int key, bool& exhausted)
{
  int victim = -1;
  exhausted = false;
  assert(_position >= 0);
  assert(0 <= key && key < XTS_SEQUENCER_MAX_KEYS);
  int64_t victimStart = 0x7FFFFFFFFFFFFFFF;
  for (int i = 0; i < XTS_SEQUENCER_MAX_VOICES; i++)
  {
    if (_started[i] == -1) return Take(key, i);
    if (_started[i] < victimStart) victimStart = _started[victim = i];
  }
  exhausted = true;
  _keys[victim] = key;
  _active[key] = victim;
  _started[victim] = _position;
  assert(0 <= victim && victim < XTS_SEQUENCER_MAX_VOICES);
  assert(0 <= _voices && _voices <= XTS_SEQUENCER_MAX_VOICES);
  return victim;
}

bool
SequencerDSP::Trigger(SequencerInput const& input)
{
  bool result = false;
  bool exhausted = false;
  for (int k = 0; k < _model->edit.keys; k++)
  {
    auto const& key = _model->pattern.rows[_row].keys[k];
    if (key.note >= PatternNote::Off)
      for (int v = 0; v < XTS_SEQUENCER_MAX_VOICES; v++)
        if (_keys[v] == k) _dsps[v].Release();
    if(key.note < PatternNote::C) continue;
    int voice = Take(k, exhausted);
    result |= exhausted;
    _synths[voice] = *_synth;
    float velocity = Param::Level(key.velocity);
    float bpm = static_cast<float>(_model->edit.bpm);
    UnitNote note = static_cast<UnitNote>(static_cast<int>(key.note) - 2);
    new (&_dsps[voice]) SynthDSP(&_synths[voice], key.octave, note, velocity, bpm, input.rate);
  }
  return result;
}

} // namespace Xts