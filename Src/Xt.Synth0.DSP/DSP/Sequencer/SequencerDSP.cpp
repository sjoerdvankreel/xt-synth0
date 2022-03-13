#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/Utility.hpp>
#include <Model/Shared/ParamInfo.hpp>
#include <DSP/Sequencer/SequencerDSP.hpp>
#include <cassert>
#include <algorithm>

namespace Xts {

void
SequencerDSP::ApplyActive()
{
  // TODO cant copy over running voices, screws up internal state
  /*
  for (int k = 0; k < MaxKeys; k++)
    if (_active[k] != -1)
      _synths[_active[k]] = *_synth;
  */
}

void
SequencerDSP::Return(int key, int voice)
{
  _voices--;
  _keys[voice] = -1;
  _started[voice] = -1;
  _endAudio = _voices == 0 && _endPattern;
  assert(0 <= key && key < XTS_SEQUENCER_MAX_KEYS);
  assert(0 <= voice && voice < MaxVoices);
  assert(0 <= _voices && _voices < MaxVoices);
}

int
SequencerDSP::Take(int key, int voice)
{
  _voices++;
  _keys[voice] = key;
  _active[key] = voice;
  _started[voice] = _pos;
  assert(0 <= _voices && _voices <= MaxVoices);
  return voice;
}

FloatSample
SequencerDSP::Next(SequencerInput const& input, bool& exhausted)
{
  exhausted = false;
  SequencerMove move = Move(input);
  if (move == SequencerMove::Next)
  {
    Automate();
    exhausted = Trigger(input);
    ApplyActive();
  } else if(move == SequencerMove::End)
  {
    for(int k = 0; k < XTS_SEQUENCER_MAX_KEYS; k++)
      if (_active[k] != -1)
        _dsps[_active[k]].Release();
  }
  FloatSample result = { 0 };
  for (int v = 0; v < MaxVoices; v++)
  {
    if (_keys[v] == -1) continue;
    _dsps[v].Next();
    result += _dsps[v].Output();
    if (_dsps[v].End()) Return(_keys[v], v);
  }
  return result;
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

void
SequencerDSP::Render(SequencerInput const& input, SequencerOutput& output)
{
  ApplyActive();
  bool exhausted;
  output.clip = false;
  output.exhausted = false;
  for (int f = 0; f < input.frames; f++)
  {
    auto out = Next(input, exhausted);
    output.exhausted |= exhausted;
    input.buffer[f * 2] = Clip(out.left, output.clip);
    input.buffer[f * 2 + 1] = Clip(out.right, output.clip);
    _pos++;
  }
  output.position = _pos;
  output.row = _row;
  output.voices = _voices;
}

SequencerMove
SequencerDSP::Move(SequencerInput const& input)
{
  if (_endPattern) return SequencerMove::None;
  int current = _row;
  int bpm = _model->edit.bpm;
  int lpb = _model->edit.lpb;
  int pats = _model->edit.patterns;
  int rows = _model->edit.rows;
  int loop = _model->edit.loop;
  if (_row == -1) return _row = 0, SequencerMove::Next;
  _fill += bpm * lpb / (60.0 * input.rate);
  if (_fill < 1.0) return SequencerMove::None;
  _fill = 0.0f;
  _row++;
  if (_row % XTS_SEQUENCER_MAX_ROWS == rows) 
  {
    _row += XTS_SEQUENCER_MAX_ROWS - rows;
    assert(_row % XTS_SEQUENCER_MAX_ROWS == 0);
  }
  if (_row == pats * XTS_SEQUENCER_MAX_ROWS)
    if (loop) _row = 0;
    else return _row = current, _endPattern = true, SequencerMove::End;
  return SequencerMove::Next;
}

void
SequencerDSP::Init(SequencerModel const* model, SynthModel const* synth, ParamBinding const* binding)
{
  _pos = 0;
  _row = -1;
  _fill = 0.0;
  _voices = 0;
  _model = model;
  _synth = synth;
  _binding = binding;
  _endAudio = false;
  _endPattern = false;
  for(int i = 0; i < XTS_SEQUENCER_MAX_KEYS; i++)
    _active[i] = -1;
  for (int i = 0; i < MaxVoices; i++)
    _started[i] = _keys[i] = -1;
}

int
SequencerDSP::Take(int key, bool& exhausted)
{
  int victim = -1;
  assert(_pos >= 0);
  exhausted = false;
  assert(0 <= key && key < XTS_SEQUENCER_MAX_KEYS);
  int64_t victimStart = 0x7FFFFFFFFFFFFFFF;
  for (int i = 0; i < MaxVoices; i++)
  {
    if (_started[i] == -1) return Take(key, i);
    if (_started[i] < victimStart)	victimStart = _started[victim = i];
  }
  exhausted = true;
  _keys[victim] = key;
  _active[key] = victim;
  _started[victim] = _pos;
  assert(0 <= victim && victim < MaxVoices);
  assert(0 <= _voices && _voices <= MaxVoices);
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
      for (int v = 0; v < MaxVoices; v++)
        if (_keys[v] == k) _dsps[v].Release();
    if (key.note >= PatternNote::C)
    {
      int voice = Take(k, exhausted);
      result |= exhausted;
      _synths[voice] = *_synth;
      float bpm = static_cast<float>(_model->edit.bpm);
      auto unote = static_cast<UnitNote>(static_cast<int>(key.note) - 2);
      new (&_dsps[voice]) SynthDSP(&_synths[voice], key.octave, unote, Param::Level(key.velocity), bpm, input.rate);
    }
  }
  return result;
}

} // namespace Xts