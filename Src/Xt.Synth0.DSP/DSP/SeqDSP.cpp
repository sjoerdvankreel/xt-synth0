#include "SeqDSP.hpp"
#include "DSP.hpp"
#include <cassert>

namespace Xts {

void
SeqDSP::Return(int key, int voice)
{
  _voices--;
  _keys[voice] = -1;
  _started[voice] = -1;
  assert(0 <= key && key < MaxKeys);
  assert(0 <= voice && voice < MaxVoices);
  assert(0 <= _voices && _voices < MaxVoices);
}

int
SeqDSP::Take(int key, int voice)
{
  _voices++;
  _keys[voice] = key;
  _started[voice] = _pos;
  assert(0 <= _voices && _voices <= MaxVoices);
  return voice;
}

bool
SeqDSP::Move(SeqInput const& input)
{
  int bpm = _model->edit.bpm;
  int lpb = _model->edit.lpb;
  int pats = _model->edit.pats;
  int rows = _model->edit.rows;
  if (_row == -1) return _row = 0, true;
  _fill += bpm * lpb / (60.0 * input.rate);
  if (_fill < 1.0) return false;
  _fill = 0.0f;
  _row++;
  if (MaxRows == rows) _row += MaxRows - rows;
  if (_row == pats * MaxRows) _row = 0;
  return true;
}

AudioOutput
SeqDSP::Next(SeqInput const& input, bool& exhausted)
{
  if (Move(input))
  {
    Automate();
    Trigger(input, exhausted);
  }
  AudioOutput result;
  for (int v = 0; v < MaxVoices; v++)
  {
    if (_keys[v] == -1) continue;
    result += _dsps[v].Next();
    if (_dsps[v].End()) Return(_keys[v], v);
  }
  return result;
}

void
SeqDSP::Automate() const
{
  for (int f = 0; f < _model->edit.fxs; f++)
  {
    auto const& fx = _model->pattern.rows[_row].fx[f];
    assert(0 <= fx.val && fx.val < 256);
    assert(0 <= fx.tgt && fx.tgt < 256);
    if (fx.tgt == 0 || fx.tgt > ParamCount) return;
    auto const& p = _model->params[fx.tgt - 1];
    if (fx.val < p.min) *p.val = p.min;
    else if (fx.val > p.max) *p.val = p.max;
    else *p.val = fx.val;
  }
}

void
SeqDSP::Render(SeqInput const& input, SeqOutput& output)
{
  bool clip;
  bool exhausted;
  output.clip = false;
  output.exhausted = false;
  for (int f = 0; f < input.frames; f++)
  {
    auto out = Next(input, exhausted);
    output.exhausted |= exhausted;
    input.buffer[f * 2] = Clip(out.l, clip);
    output.clip |= clip;
    input.buffer[f * 2 + 1] = Clip(out.r, clip);
    output.clip |= clip;
    _pos++;
  }
  output.pos = _pos;
  output.row = _row;
  output.voices = _voices;
}

void
SeqDSP::Init(SeqModel const* model, SynthModel const* synth)
{
  _pos = 0;
  _row = -1;
  _fill = 0.0;
  _voices = 0;
  _model = model;
  _synth = synth;
  for (int i = 0; i < MaxVoices; i++)
    _started[i] = _keys[i] = -1;
}

void
SeqDSP::Trigger(SeqInput const& input, bool& exhausted)
{
  for (int k = 0; k < _model->edit.keys; k++)
  {
    auto const& key = _model->pattern.rows[_row].keys[k];
    auto note = static_cast<PatternNote>(key.note);
    if (note >= PatternNote::Off)
      for (int v = 0; v < MaxVoices; v++)
        if (_keys[v] == k) _dsps[v].Release();
    if (note >= PatternNote::C)
    {
      int voice = Take(k, exhausted);
      float bpm = static_cast<float>(_model->edit.bpm);
      _synths[voice] = *_synth;
      auto unote = static_cast<UnitNote>(static_cast<int>(note) - 2);
      _inputs[voice] = AudioInput(input.rate, bpm, key.oct, unote);
      _dsps[voice] = SynthDSP(&_synth[voice], &_inputs[voice]);
    }
  }
}

int
SeqDSP::Take(int key, bool& exhausted)
{
  int victim = -1;
  assert(_pos >= 0);
  exhausted = false;
  assert(0 <= key && key < MaxKeys);
  int64_t victimStart = 0x7FFFFFFFFFFFFFFF;
  for (int i = 0; i < MaxVoices; i++)
  {
    if (_started[i] == -1) return Take(key, i);
    if (_started[i] < victimStart)	victimStart = _started[victim = i];
  }
  exhausted = true;
  _keys[victim] = key;
  _started[victim] = _pos;
  assert(0 <= victim && victim < MaxVoices);
  assert(0 <= _voices && _voices <= MaxVoices);
  return victim;
}

} // namespace Xts