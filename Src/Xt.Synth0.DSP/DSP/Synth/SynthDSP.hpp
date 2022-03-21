#ifndef XTS_DSP_SYNTH_SYNTH_DSP_HPP
#define XTS_DSP_SYNTH_SYNTH_DSP_HPP

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
  void ReleaseAll();
  FloatSample Next();
  void Release(int key);
  void Automate(int target, int value);
  bool Trigger(int key, int octave, UnitNote note, float velocity, int64_t position);
public:
  SynthDSP() = default;
  SynthDSP(SynthDSP const&) = default;
  SynthDSP(SynthModel const* model, ParamBinding const* binding, int fxCount, int keyCount, float bpm, float rate);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_SYNTH_DSP_HPP