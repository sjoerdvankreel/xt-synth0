#include <DSP/Synth/SynthDSP.hpp>
#include <Model/Synth/SynthModel.hpp>

#include <memory>
#include <cassert>
#include <algorithm>

namespace Xts {

StagedParams
SynthPlot::Params() const
{
  StagedParams result;
  result.stereo = true;
  result.bipolar = true;
  result.allowSpectrum = true;
  result.allowResample = false;
  return result;
}

void
SynthPlot::Render(SynthModel const& model, PlotInput const& input, PlotState& state)
{ std::make_unique<SynthPlot>()->DoRender(input, state); }

void
SynthDSP::Init()
{ new(&_globalLfo) LfoDSP(&_model.global.lfo, _bpm, _rate); }

FloatSample
SynthDSP::Next()
{
  FloatSample result = { 0 };
  CvSample globalLfo = _globalLfo.Next();
  for (int v = 0; v < XTS_SYNTH_MAX_VOICES; v++)
  {
    if (_voiceKeys[v] == -1) continue;
    result += _voiceDsps[v].Next(globalLfo);
    if (_voiceDsps[v].End()) Return(_voiceKeys[v], v);
  }
  return result;
}

void
SynthDSP::Release(int key)
{
  for (int v = 0; v < XTS_SYNTH_MAX_VOICES; v++)
    if (_voiceKeys[v] == key) _voiceDsps[v].Release();
}

void SynthDSP::ReleaseAll()
{
  for (int k = 0; k < _keyCount; k++)
    if (_voicesActive[k] != -1) _voiceDsps[_voicesActive[k]].Release();
}

void
SynthDSP::Automate(int target, int value)
{
  assert(0 <= value && value < 256);
  assert(0 <= target && target < 255);
  if (target >= XTS_SYNTH_PARAM_COUNT) return;
  int32_t* param = _binding[target];
  ParamInfo const& info = SynthModel::Params()[target];
  *param = std::clamp(value, info.min, info.max);
}

void
SynthDSP::Return(int key, int voice)
{
  _voices--;
  _voiceKeys[voice] = -1;
  _voicesStarted[voice] = -1;
  assert(0 <= key && key < _keyCount);
  assert(0 <= voice && voice < XTS_SYNTH_MAX_VOICES);
  assert(0 <= _voices && _voices < XTS_SYNTH_MAX_VOICES);
}

int
SynthDSP::Take(int key, int voice, int64_t position)
{
  _voices++;
  _voiceKeys[voice] = key;
  _voicesActive[key] = voice;
  _voicesStarted[voice] = position;
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
    if (_voicesStarted[i] == -1) return Take(key, i, position);
    if (_voicesStarted[i] < victimStart) victimStart = _voicesStarted[victim = i];
  }
  exhausted = true;
  _voiceKeys[victim] = key;
  _voicesActive[key] = victim;
  _voicesStarted[victim] = position;
  assert(0 <= victim && victim < XTS_SYNTH_MAX_VOICES);
  assert(0 <= _voices && _voices <= XTS_SYNTH_MAX_VOICES);
  return victim;
}

bool
SynthDSP::Trigger(int key, int octave, UnitNote note, float velocity, int64_t position)
{
  bool result = false;
  int voice = Take(key, position, result);
  _voiceModels[voice] = _model;
  new (&_voiceDsps[voice]) VoiceDSP(&_voiceModels[voice].voice, octave, note, velocity, _bpm, _rate);
  return result;
}

SynthDSP::
SynthDSP(int fxCount, int keyCount, float bpm, float rate) :
  SynthDSP()
{
  _bpm = bpm;
  _voices = 0;
  _rate = rate;
  _fxCount = fxCount;
  _keyCount = keyCount;
  for (int i = 0; i < keyCount; i++) _voicesActive[i] = -1;
  for (int i = 0; i < XTS_SYNTH_MAX_VOICES; i++) _voicesStarted[i] = _voiceKeys[i] = -1;
}

} // namespace Xts