#ifndef XTS_DSP_SYNTH_SYNTH_DSP_HPP
#define XTS_DSP_SYNTH_SYNTH_DSP_HPP

#include <DSP/Plot.hpp>
#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Synth/AmpDSP.hpp>
#include <DSP/Synth/AudioDSP.hpp>
#include <Model/Synth/UnitModel.hpp>

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
  bool End() const { return _cv.End(_amp.Env()); }
  FloatSample Output() const { return _amp.Output(); }
  EnvModel const& AmpEnv() const { return _cv.}
  EnvSample Release() { return _cv.ReleaseAll(_amp.Env()); }
  EnvSample EnvOutput() const { return _cv.EnvOutput(_amp.Env()); }
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
  StagedParams Params() const;
  void Init(float bpm, float rate);
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotOutput& output);
public:
  void Next() { _dsp.Next(); }
  bool End() const { return _dsp.End(); }
  EnvSample Release() { return _dsp.Release(); }
  float Left() const { return _dsp.Output().left; }
  float Right() const { return _dsp.Output().right; }
  EnvSample EnvOutput() const { return _dsp.EnvOutput(); }
  float ReleaseSamples(float bpm, float rate) const { return EnvPlot::ReleaseSamples(_cv->envs[_ampDsp.Env()], bpm, rate); }
};

auto next = [](SynthDSP& dsp) { dsp.Next(); };
auto end = [](SynthDSP const& dsp) { return dsp.End(); };
auto release = [](SynthDSP& dsp) { return dsp.Release(); };
auto left = [](SynthDSP const& dsp) { return dsp.Output().left; };
auto right = [](SynthDSP const& dsp) { return dsp.Output().right; };
auto envOutput = [](SynthDSP const& dsp) { return dsp._cv.EnvOutput(dsp._amp.Env()); };
auto factory = [&](float rate) { return SynthDSP(&model, 4, UnitNote::C, 1.0f, input.bpm, rate); };

} // namespace Xts
#endif // XTS_DSP_SYNTH_SYNTH_DSP_HPP