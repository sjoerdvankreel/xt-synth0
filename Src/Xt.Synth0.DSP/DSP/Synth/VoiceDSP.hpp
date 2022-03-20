#ifndef XTS_DSP_SYNTH_VOICE_DSP_HPP
#define XTS_DSP_SYNTH_VOICE_DSP_HPP

#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Synth/AmpDSP.hpp>
#include <DSP/Synth/AudioDSP.hpp>
#include <Model/Synth/SynthModel.hpp>

namespace Xts {

class VoiceDSP
{
  CvDSP _cv;
  AmpDSP _amp;
  AudioDSP _audio;
public:
  VoiceDSP() = default;
  VoiceDSP(struct SynthModel const* model, int oct, UnitNote note, float velocity, float bpm, float rate);
public:
  FloatSample Output() const { return _amp.Output(); }
  bool End() const { return _cv.Env(XTS_AMP_ENV).End(); }
  EnvSample Release() { return _cv.ReleaseAll(XTS_AMP_ENV); }
  EnvModel const& Env() const { return _cv.Env(XTS_AMP_ENV).Model(); }
  EnvSample EnvOutput() const { return _cv.Env(XTS_AMP_ENV).Output(); }
  FloatSample Next(CvState const& cv, AudioState const& audio) { return _amp.Next(cv, audio); };
  FloatSample Next() { _cv.Next(); _audio.Next(_cv.Output()); return Next(_cv.Output(), _audio.Output()); }
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_VOICE_DSP_HPP