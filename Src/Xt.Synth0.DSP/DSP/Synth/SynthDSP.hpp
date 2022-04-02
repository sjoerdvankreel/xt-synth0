#ifndef XTS_DSP_SYNTH_SYNTH_DSP_HPP
#define XTS_DSP_SYNTH_SYNTH_DSP_HPP

#include <DSP/Synth/LfoDSP.hpp>
#include <DSP/Synth/VoiceDSP.hpp>
#include <DSP/Synth/DelayDSP.hpp>
#include <DSP/Synth/MasterDSP.hpp>
#include <DSP/Synth/GlobalFilterDSP.hpp>
#include <Model/Synth/SynthModel.hpp>
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
  DelayDSP _delay;
  SynthModel _model;
  LfoDSP _globalLfo;
  MasterDSP _master;
  FloatSample _output;
  GlobalFilterDSP _globalFilter;
  int* _binding[XTS_SYNTH_PARAM_COUNT];
  int _voiceKeys[XTS_SYNTH_MAX_VOICES];
  int _voicesActive[XTS_SHARED_MAX_KEYS];
  VoiceDSP _voiceDsps[XTS_SYNTH_MAX_VOICES];
  int64_t _voicesStarted[XTS_SYNTH_MAX_VOICES];
  SynthModel _voiceModels[XTS_SYNTH_MAX_VOICES];
  int* _voiceBindings[XTS_SYNTH_MAX_VOICES][XTS_SYNTH_PARAM_COUNT];
public:
  int** Binding() { return _binding; }
  int Voices() const { return _voices; }
  SynthModel* Model() { return &_model; }
  SynthModel* VoiceModels() { return _voiceModels; }
  int** VoiceBindings() { return _voiceBindings[0]; }
public:
  SynthDSP() = default;
  SynthDSP(SynthDSP const&) = default;
  SynthDSP(int keyCount, float bpm, float rate);
private:
  void Return(int key, int voice);
  int Take(int key, int voice, int64_t position);
  int Take(int key, int64_t position, bool& exhausted);
public:
  VoiceDSP& Voice0() { return _voiceDsps[0]; };
  FloatSample Output() const { return _output; }
  DelayDSP const& Delay() const { return _delay; }
  MasterDSP const& Master() const { return _master; }
  VoiceDSP const& Voice0() const { return _voiceDsps[0]; }
  GlobalFilterDSP const& GlobalFilter() const { return _globalFilter; }
public:
  void Init();
  void ReleaseAll();
  FloatSample Next();
  void Release(int key);
  void Automate(int target, int value, int64_t position);
  bool Trigger(int key, int octave, NoteType note, float velocity, int64_t position);
  static void RenderPlot(struct SynthModel const& model, struct PlotInput const& input, struct PlotState& state);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_SYNTH_DSP_HPP