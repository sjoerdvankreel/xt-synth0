#ifndef XTS_DSP_SYNTH_GLOBAL_DSP_HPP
#define XTS_DSP_SYNTH_GLOBAL_DSP_HPP

#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Synth/ModDSP.hpp>
#include <DSP/Synth/CvState.hpp>
#include <DSP/Synth/AudioState.hpp>
#include <DSP/Shared/Plot.hpp>
#include <DSP/Shared/AudioSample.hpp>
#include <Model/Synth/GlobalModel.hpp>
#include <Model/Synth/SynthConfig.hpp>

namespace Xts {

class GlobalDSP
{
  float _amp;
  float _level;
  float _panning;
  ModDSP _ampMod;
  ModDSP _panMod;
  FloatSample _output;
  GlobalModel const* _model;
  float _unitAmount[XTS_SYNTH_UNIT_COUNT];
  float _filterAmount[XTS_SYNTH_FILTER_COUNT];
public:
  GlobalDSP() = default;
  GlobalDSP(GlobalModel const* model, float velocity);
public:
  float Level() const { return _level; }
  FloatSample Output() const { return _output; };
  FloatSample Next(CvState const& cv, AudioState const& audio);
  int Env() const { return static_cast<int>(_model->ampEnvSource); };
};

class GlobalPlot: 
public StagedPlot
{
  CvDSP _cvDsp;
  GlobalDSP _globalDsp;
  CvModel const* _cv;
  GlobalModel const* _global;
public:
  GlobalPlot(CvModel const* cv, GlobalModel const* global);
public:
  float Right() const { return 0.0f; }
  float Left() const { return _globalDsp.Level(); }
  void Next() { _globalDsp.Next(_cvDsp.Next(), {}); }
  bool End() const { return _cvDsp.Env(_globalDsp.Env()).End(); }
  EnvSample Release() { return _cvDsp.ReleaseAll(_globalDsp.Env()); };
  EnvSample EnvOutput() const { return _cvDsp.Env(_globalDsp.Env()).Output(); }
public:
  StagedParams Params() const;
  void Init(float bpm, float rate);
  float ReleaseSamples(float bpm, float rate) const;
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotState& state);
};

inline GlobalPlot::
GlobalPlot(CvModel const* cv, GlobalModel const* global):
_cv(cv), _global(global) {}

inline float
GlobalPlot::ReleaseSamples(float bpm, float rate) const
{ return EnvPlot::ReleaseSamples(_cv->envs[_globalDsp.Env()], bpm, rate); }

} // namespace Xts
#endif // XTS_DSP_SYNTH_GLOBAL_DSP_HPP