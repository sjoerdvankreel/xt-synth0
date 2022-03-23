#ifndef XTS_DSP_SYNTH_VOICE_DSP_HPP
#define XTS_DSP_SYNTH_VOICE_DSP_HPP

#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Synth/AmpDSP.hpp>
#include <DSP/Synth/AudioDSP.hpp>
#include <Model/Synth/SynthModel.hpp>
#include <Model/Synth/SynthConfig.hpp>
#include <vector>

namespace Xts {

class VoiceDSP
{
  CvDSP _cv;
  AmpDSP _amp;
  AudioDSP _audio;
  SynthModel _model;
  std::vector<int*> _binding;
public:
  VoiceDSP() = default;
  VoiceDSP(int oct, UnitNote note, float velocity, float bpm, float rate);
public:
  SynthModel* Model() { return &_model; }
  int** Binding() { return _binding.data(); }
  FloatSample Output() const { return _amp.Output(); }
  bool End() const { return _cv.Env(XTS_AMP_ENV).End(); }
  EnvSample Release() { return _cv.ReleaseAll(XTS_AMP_ENV); }
  EnvModel const& Env() const { return _cv.Env(XTS_AMP_ENV).Model(); }
  EnvSample EnvOutput() const { return _cv.Env(XTS_AMP_ENV).Output(); }
  FloatSample Next(CvState const& cv, AudioState const& audio) { return _amp.Next(cv, audio); };
  FloatSample Next(CvSample globalLfo) { _cv.Next(globalLfo); _audio.Next(_cv.Output()); return Next(_cv.Output(), _audio.Output()); }
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_VOICE_DSP_HPP