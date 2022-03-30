#ifndef XTS_DSP_SYNTH_GLOBAL_PLOT_HPP
#define XTS_DSP_SYNTH_GLOBAL_PLOT_HPP

#include <DSP/Shared/Plot.hpp>
#include <DSP/Synth/SynthDSP.hpp>
#include <cstdint>

namespace Xts {

class GlobalPlot: 
public StagedPlot
{
  struct SynthModel const* _model;
protected:
  SynthDSP _dsp;
public:
  GlobalPlot(struct SynthModel const* model) : _model(model) {}
public:
  StagedParams Params() const;
public:
  void Next() { _dsp.Next(); }
  bool End() const { return _dsp.Voice0().End(); }
  EnvSample Release() { return _dsp.Voice0().Release(); }
  void Start() { _dsp.Trigger(0, 4, UnitNote::C, 1.0f, 0); }
  EnvSample EnvOutput() const { return _dsp.Voice0().EnvOutput(); }
  void Init(float bpm, float rate) { new(&_dsp) SynthDSP(1, bpm, rate); *_dsp.Model() = *_model; _dsp.Init(); }
  float ReleaseSamples(float bpm, float rate) const { return EnvPlot::ReleaseSamples(_dsp.Voice0().Env(), bpm, rate); }
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_GLOBAL_PLOT_HPP