#include <DSP/Synth/SynthDSP.hpp>
#include <Model/Synth/SynthModel.hpp>
#include <Model/Shared/ParamBinding.hpp>

#include <cassert>
#include <algorithm>

namespace Xts {

FloatSample
SynthDSP::Next()
{
  FloatSample result = { 0 };
  for (int v = 0; v < XTS_SYNTH_MAX_VOICES; v++)
  {
    if (_keys[v] == -1) continue;
    result += _dsps[v].Next();
    if (_dsps[v].End()) Return(_keys[v], v);
  }
  return result;
}

void
SynthDSP::Release(int key)
{
  for (int v = 0; v < XTS_SYNTH_MAX_VOICES; v++)
    if (_keys[v] == key) _dsps[v].Release();
}

void SynthDSP::ReleaseAll()
{
  for (int k = 0; k < _keyCount; k++)
    if (_active[k] != -1) _dsps[_active[k]].Release();
}

void
SynthDSP::Automate(int target, int value)
{
  assert(0 <= value && value < 256);
  assert(0 <= target && target < 256);
  if (target == 0 || target > XTS_SYNTH_PARAM_COUNT) return;
  int32_t* param = _binding->params[target - 1];
  ParamInfo const& info = SynthModel::Params()[target - 1];
  *param = std::clamp(value, info.min, info.max);
}

void
SynthDSP::Return(int key, int voice)
{
  _voices--;
  _keys[voice] = -1;
  _started[voice] = -1;
  assert(0 <= key && key < _keyCount);
  assert(0 <= voice && voice < XTS_SYNTH_MAX_VOICES);
  assert(0 <= _voices && _voices < XTS_SYNTH_MAX_VOICES);
}

int
SynthDSP::Take(int key, int voice, int64_t position)
{
  _voices++;
  _keys[voice] = key;
  _active[key] = voice;
  _started[voice] = position;
  assert(0 <= _voices && _voices <= XTS_SYNTH_MAX_VOICES);
  return voice;
}

int
SynthDSP::Take(int key, int64_t position, bool& exhausted)
{
  int victim = -1;
  assert(position >= 0);
  assert(0 <= key && key < _keyCount);
  int64_t victimStart = 0x7FFFFFFFFFFFFFFF;
  for (int i = 0; i < XTS_SYNTH_MAX_VOICES; i++)
  {
    if (_started[i] == -1) return Take(key, i, position);
    if (_started[i] < victimStart) victimStart = _started[victim = i];
  }
  exhausted = true;
  _keys[victim] = key;
  _active[key] = victim;
  _started[victim] = position;
  assert(0 <= victim && victim < XTS_SYNTH_MAX_VOICES);
  assert(0 <= _voices && _voices <= XTS_SYNTH_MAX_VOICES);
  return victim;
}

bool
SynthDSP::Trigger(int key, int octave, UnitNote note, float velocity, int64_t position)
{
  bool result = false;
  int voice = Take(key, position, result);
  _synths[voice] = *_model;
  new (&_dsps[voice]) VoiceDSP(&_synths[voice], octave, note, velocity, _bpm, _rate);
  return result;
}

SynthDSP::
SynthDSP(SynthModel const* model, ParamBinding const* binding, int fxCount, int keyCount, float bpm, float rate) :
  SynthDSP()
{
  _bpm = bpm;
  _voices = 0;
  _rate = rate;
  _model = model;
  _binding = binding;
  _fxCount = fxCount;
  _keyCount = keyCount;
  for (int i = 0; i < keyCount; i++) _active[i] = -1;
  for (int i = 0; i < XTS_SYNTH_MAX_VOICES; i++) _started[i] = _keys[i] = -1;
}

} // namespace Xts