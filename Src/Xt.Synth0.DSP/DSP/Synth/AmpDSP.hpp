#ifndef XTS_DSP_SYNTH_AMP_DSP_HPP
#define XTS_DSP_SYNTH_AMP_DSP_HPP

#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Synth/CvState.hpp>
#include <DSP/Synth/AudioState.hpp>
#include <DSP/Synth/VoiceModDSP.hpp>
#include <DSP/Shared/Plot.hpp>
#include <DSP/Shared/AudioSample.hpp>
#include <Model/Synth/AmpModel.hpp>
#include <Model/Synth/SynthConfig.hpp>

#define XTS_AMP_ENV 0

namespace Xts {

class AmpDSP
{
  float _level;
  float _velocity;
  VoiceModDSP _ampMod;
  VoiceModDSP _panMod;
  FloatSample _output;
  AmpModel const* _model;
public:
  AmpDSP() = default;
  AmpDSP(AmpModel const* model, float velocity);
public:
  float Level() const { return _level; }
  FloatSample Output() const { return _output; };
  FloatSample Next(CvState const& cv, AudioState const& audio);
};

class AmpPlot: 
public StagedPlot
{
  CvDSP _cvDsp;
  AmpDSP _ampDsp;
  LfoDSP _globalLfoDsp;
  struct SynthModel const* _model;
public:
  AmpPlot(struct SynthModel const* model): _model(model) {}
public:
  void Start() {}
  float Right() const { return 0.0f; }
  float Left() const { return _ampDsp.Level(); }
  bool End() const { return _cvDsp.Env(XTS_AMP_ENV).End(); }
  EnvSample Release() { return _cvDsp.ReleaseAll(XTS_AMP_ENV); };
  void Next() { _ampDsp.Next(_cvDsp.Next(_globalLfoDsp.Next()), {}); }
  EnvSample EnvOutput() const { return _cvDsp.Env(XTS_AMP_ENV).Output(); }
public:
  StagedParams Params() const;
  void Init(float bpm, float rate);
  float ReleaseSamples(float bpm, float rate) const;
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotState& state);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_AMP_DSP_HPP