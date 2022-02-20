#ifndef XTS_SYNTH_DSP_HPP
#define XTS_SYNTH_DSP_HPP

#include "CvDSP.hpp"
#include "AmpDSP.hpp"
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
  void Release() { _cv.Release(); }
  bool End() const { return _amp.End(_cv); }
  AudioOutput Output() const { return _amp.Output(); }
  AudioOutput Next(CvState const& cv, AudioState const& audio) { return _amp.Next(cv, audio); };
  AudioOutput Next() { _cv.Next(); _audio.Next(_cv.Output()); return Next(_cv.Output(), _audio.Output()); }
  static void Plot(SynthModel const& model, PlotInput const& input, PlotOutput& output);
};

} // namespace Xts
#endif // XTS_SYNTH_DSP_HPP