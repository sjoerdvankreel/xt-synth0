#ifndef XTS_DSP_SYNTH_SYNTH_DSP_HPP
#define XTS_DSP_SYNTH_SYNTH_DSP_HPP

#include <DSP/Shared/Plot.hpp>
#include <DSP/Synth/VoiceDSP.hpp>
#include <Model/Synth/SynthConfig.hpp>
#include <Model/Shared/SharedConfig.hpp>
#include <cstdint>

namespace Xts {

class SynthDSP 
{
  float _bpm;
  float _rate;
  int _voices;
  int _fxCount;
  int _keyCount;
  struct SynthModel const* _model;
  struct ParamBinding const* _binding;
  int _voiceKeys[XTS_SYNTH_MAX_VOICES];
  int _voicesActive[XTS_SHARED_MAX_KEYS];
  VoiceDSP _voiceDsps[XTS_SYNTH_MAX_VOICES];
  int64_t _voicesStarted[XTS_SYNTH_MAX_VOICES];
  VoiceModel _voiceModels[XTS_SYNTH_MAX_VOICES];
public:
  int Voices() const { return _voices; }
private:
  void Return(int key, int voice);
  int Take(int key, int voice, int64_t position);
  int Take(int key, int64_t position, bool& exhausted);
public:
  VoiceDSP& Voice0() { return _voiceDsps[0]; };
  VoiceDSP const& Voice0() const { return _voiceDsps[0]; }
public:
  void ReleaseAll();
  FloatSample Next();
  void Release(int key);
  void Automate(int target, int value);
  bool Trigger(int key, int octave, UnitNote note, float velocity, int64_t position);
public:
  SynthDSP() = default;
  SynthDSP(SynthDSP const&) = default;
  SynthDSP(struct SynthModel const* model, struct ParamBinding const* binding, int fxCount, int keyCount, float bpm, float rate);
};

class SynthPlot: 
public StagedPlot
{
  SynthDSP _dsp;
  struct SynthModel const* _model;
  struct ParamBinding const* _binding;
public:
  SynthPlot(struct SynthModel const* model) : _model(model) {}
public:
  StagedParams Params() const;
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotState& state);
public:
  void Next() { _dsp.Next(); }
  bool End() const { return _dsp.Voice0().End(); }
  EnvSample Release() { return _dsp.Voice0().Release(); }
  float Left() const { return _dsp.Voice0().Output().left; }
  float Right() const { return _dsp.Voice0().Output().right; }
  EnvSample EnvOutput() const { return _dsp.Voice0().EnvOutput(); }
  void Init(float bpm, float rate) { new(&_dsp) SynthDSP(_model, _binding, 0, 1, bpm, rate); }
  float ReleaseSamples(float bpm, float rate) const { return EnvPlot::ReleaseSamples(_dsp.Voice0().Env(), bpm, rate); }
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_SYNTH_DSP_HPP