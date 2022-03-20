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
  int _keys[XTS_SYNTH_MAX_VOICES];
  int _active[XTS_SHARED_MAX_KEYS];
  VoiceDSP _dsps[XTS_SYNTH_MAX_VOICES];
  int64_t _started[XTS_SYNTH_MAX_VOICES];
  SynthModel _synths[XTS_SYNTH_MAX_VOICES];
private:
  FloatSample Next(float rate);
  void Return(int key, int voice);
  void Automate(int target, int value);
  int Take(int key, int voice, int64_t position);
  int Take(int key, int64_t position, bool& exhausted);
public:
  void Release(int key);
  bool Trigger(int key, int octave, UnitNote note, float velocity, int64_t position);
public:
  SynthDSP() = default;
  SynthDSP(SynthModel const* model, ParamBinding const* binding, int fxCount, int keyCount, float bpm, float rate);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_SYNTH_DSP_HPP