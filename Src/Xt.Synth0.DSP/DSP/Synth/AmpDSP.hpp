#ifndef XTS_DSP_SYNTH_AMP_DSP_HPP
#define XTS_DSP_SYNTH_AMP_DSP_HPP

#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Synth/ModDSP.hpp>
#include <DSP/Synth/CvState.hpp>
#include <DSP/Synth/AudioState.hpp>
#include <DSP/Shared/Plot.hpp>
#include <DSP/Shared/AudioSample.hpp>
#include <Model/Synth/AmpModel.hpp>
#include <Model/Synth/SynthConfig.hpp>

namespace Xts {

class AmpDSP
{
  float _amp;
  float _level;
  float _panning;
  ModDSP _ampMod;
  ModDSP _panMod;
  FloatSample _output;
  AmpModel const* _model;
  float _unitAmount[XTS_SYNTH_UNIT_COUNT];
  float _filterAmount[XTS_SYNTH_FILTER_COUNT];
public:
  AmpDSP() = default;
  AmpDSP(AmpModel const* model, float velocity);
public:
  float Level() const { return _level; }
  FloatSample Output() const { return _output; };
  FloatSample Next(CvState const& cv, AudioState const& audio);
  int Env() const { return static_cast<int>(_model->ampEnvSource); };
};

class AmpPlot: 
public StagedPlot
{
  CvDSP _cvDsp;
  AmpDSP _ampDsp;
  CvModel const* _cv;
  AmpModel const* _amp;
public:
  AmpPlot(CvModel const* cv, AmpModel const* amp);
public:
  float Right() const { return 0.0f; }
  float Left() const { return _ampDsp.Level(); }
  void Next() { _ampDsp.Next(_cvDsp.Next(), {}); }
  bool End() const { return _cvDsp.Env(_ampDsp.Env()).End(); }
  EnvSample Release() { return _cvDsp.ReleaseAll(_ampDsp.Env()); };
  EnvSample EnvOutput() const { return _cvDsp.Env(_ampDsp.Env()).Output(); }
public:
  StagedParams Params() const;
  void Init(float bpm, float rate);
  float ReleaseSamples(float bpm, float rate) const;
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotOutput& output);
};

inline AmpPlot::
AmpPlot(CvModel const* cv, AmpModel const* amp): 
_cv(cv), _amp(amp) {}

inline float
AmpPlot::ReleaseSamples(float bpm, float rate) const
{ return EnvPlot::ReleaseSamples(_cv->envs[_ampDsp.Env()], bpm, rate); }

} // namespace Xts
#endif // XTS_DSP_SYNTH_AMP_DSP_HPP