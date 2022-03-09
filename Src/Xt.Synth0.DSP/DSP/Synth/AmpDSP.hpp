#ifndef XTS_DSP_SYNTH_AMP_DSP_HPP
#define XTS_DSP_SYNTH_AMP_DSP_HPP

#include <DSP/Plot.hpp>
#include <DSP/AudioSample.hpp>
#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Synth/ModDSP.hpp>
#include <DSP/Synth/CvState.hpp>
#include <DSP/Synth/AudioState.hpp>
#include <Model/Synth/Config.hpp>

namespace Xts {

class AmpDSP
{
  float _amp;
  float _level;
  float _panning;
  ModDSP _ampMod;
  ModDSP _panMod;
  FloatSample _output;
  struct AmpModel const* _model;
  float _unitAmount[XTS_SYNTH_UNIT_COUNT];
  float _filterAmount[XTS_SYNTH_FILTER_COUNT];
public:
  AmpDSP() = default;
  AmpDSP(struct AmpModel const* model, float velocity);
public:
  float Level() const { return _level; }
  FloatSample Output() const { return _output; };
  int Env() const { return static_cast<int>(_model->ampEnvSource); };
  FloatSample Next(struct CvState const& cv, struct AudioState const& audio);
};

class AmpPlot: 
public StagedPlot
{
  CvDSP _cvDsp;
  AmpDSP _ampDsp;
  struct CvModel const* _cv;
  struct AmpModel const* _amp;
public:
  AmpPlot(struct CvModel const* cv, struct AmpModel const* amp) : _cv(cv), _amp(amp) {}
public:
  StagedParams Params() const;
  void Init(float bpm, float rate);
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotOutput& output);
public:
  float Right() const { return 0.0f; }
  float Left() const { return _ampDsp.Level(); }
  void Next() { _ampDsp.Next(_cvDsp.Next(), {}); }
  bool End() const { return _cvDsp.End(_ampDsp.Env()); }
  EnvSample Release() { return _cvDsp.ReleaseAll(_ampDsp.Env()); };
  EnvSample EnvOutput() const { return _cvDsp.EnvOutput(_ampDsp.Env()); }
  float ReleaseSamples(float bpm, float rate) const { return EnvPlot::ReleaseSamples(_cv->envs[_ampDsp.Env()], bpm, rate); }
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_AMP_DSP_HPP