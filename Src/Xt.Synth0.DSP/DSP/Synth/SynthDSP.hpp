#ifndef XTS_DSP_SYNTH_SYNTH_DSP_HPP
#define XTS_DSP_SYNTH_SYNTH_DSP_HPP

#include <DSP/Plot.hpp>
#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Synth/AmpDSP.hpp>
#include <DSP/Synth/AudioDSP.hpp>
#include <Model/Synth/SynthModel.hpp>

namespace Xts {

class SynthDSP
{
  CvDSP _cv;
  AmpDSP _amp;
  AudioDSP _audio;
public:
  SynthDSP() = default;
  SynthDSP(struct SynthModel const* model, int oct, UnitNote note, float velocity, float bpm, float rate);
public:
  FloatSample Output() const { return _amp.Output(); }
  bool End() const { return _cv.Env(_amp.Env()).End(); }
  EnvSample Release() { return _cv.ReleaseAll(_amp.Env()); }
  EnvModel const& Env() const { return _cv.Env(_amp.Env()).Model(); }
  EnvSample EnvOutput() const { return _cv.Env(_amp.Env()).Output(); }
  FloatSample Next(CvState const& cv, AudioState const& audio) { return _amp.Next(cv, audio); };
  FloatSample Next() { _cv.Next(); _audio.Next(_cv.Output()); return Next(_cv.Output(), _audio.Output()); }
};

class SynthPlot: 
public StagedPlot
{
  SynthDSP _dsp;
  struct SynthModel const* _model;
public:
  SynthPlot(struct SynthModel const* model) : _model(model) {}
public:
  void Next() { _dsp.Next(); }
  bool End() const { return _dsp.End(); }
  EnvSample Release() { return _dsp.Release(); }
  float Left() const { return _dsp.Output().left; }
  float Right() const { return _dsp.Output().right; }
  EnvSample EnvOutput() const { return _dsp.EnvOutput(); }
  float ReleaseSamples(float bpm, float rate) const { return EnvPlot::ReleaseSamples(_dsp.Env(), bpm, rate); }
public:
  StagedParams Params() const;
  void Init(float bpm, float rate) { new(&_dsp) SynthDSP(_model, 4, UnitNote::C, 1.0f, bpm, rate); }
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotOutput& output);
};

inline void
SynthPlot::Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotOutput& output)
{ SynthPlot(&model).RenderCore(input, model.plot.hold, output); }

} // namespace Xts
#endif // XTS_DSP_SYNTH_SYNTH_DSP_HPP