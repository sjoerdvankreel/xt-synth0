#ifndef XTS_SYNTH_DSP_HPP
#define XTS_SYNTH_DSP_HPP

#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Synth/AmpDSP.hpp>
#include "AudioDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class SynthDSP
{
  CvDSP _cv;
  AmpDSP _amp;
  AudioDSP _audio;
public:
  SynthDSP() = default;
  SynthDSP(SynthModel const* model, int oct, UnitNote note, float velo, float bpm, float rate);
public:
  bool End() const { return _cv.End(_amp.Env()); }
  FloatSample Output() const { return _amp.Output(); }
  EnvSample Release() { return _cv.ReleaseAll(_amp.Env()); }
  FloatSample Next(CvState const& cv, AudioState const& audio) { return _amp.Next(cv, audio); };
  FloatSample Next() { _cv.Next(); _audio.Next(_cv.Output()); return Next(_cv.Output(), _audio.Output()); }
  static void Plot(SynthModel const& model, EnvModel const& envModel, bool spec, int hold, PlotInput const& input, PlotOutput& output);
};

} // namespace Xts
#endif // XTS_SYNTH_DSP_HPP