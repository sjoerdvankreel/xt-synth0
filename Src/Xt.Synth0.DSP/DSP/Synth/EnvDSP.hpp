#ifndef XTS_DSP_SYNTH_ENV_DSP_HPP
#define XTS_DSP_SYNTH_ENV_DSP_HPP

#include <DSP/Shared/Plot.hpp>
#include <DSP/Shared/EnvSample.hpp>
#include <Model/Synth/EnvModel.hpp>

namespace Xts {

struct EnvParams
{
  float sustain;
  int holdSamples;
  int delaySamples;
  int decaySamples;
  int attackSamples;
  int releaseSamples;
};

class EnvDSP
{
  int _pos;
  float _max;
  double _base;
  double _increment;
  EnvSample _output;
  EnvParams _params;
  EnvModel const* _model;
public:
  EnvDSP() = default;
  EnvDSP(EnvModel const* model, float bpm, float rate);
public:
  EnvSample Next();
  EnvSample Release();
  EnvSample Output() const;
  EnvModel const& Model() const { return *_model; }
  bool End() const { return _output.stage == EnvStage::End; }
private:
  float Generate();
  void CycleStage(EnvType type);
  void NextStage(EnvStage stage);
  float Generate(float from, float to, SlopeType type);
  static EnvParams Params(EnvModel const& model, float bpm, float rate);
};

class EnvPlot: 
public StagedPlot
{
  EnvDSP _dsp;
  EnvModel const* _model;
public:
  EnvPlot(EnvModel const* model): _model(model) {}
public:
  void Start() {}
  void Next() { _dsp.Next(); };
  float Right() const { return 0.0f; }
  bool End() const { return _dsp.End(); }
  EnvSample Release() { return _dsp.Release(); };
  float Left() const { return _dsp.Output().value; }
  EnvSample EnvOutput() const { return _dsp.Output(); }
  void Init(int lpb, float bpm, float rate) { _dsp = EnvDSP(_model, bpm, rate); }
  float ReleaseSamples(float bpm, float rate) const { return ReleaseSamples(*_model, bpm, rate); }
public:
  StagedParams Params() const;
  static float ReleaseSamples(EnvModel const& model, float bpm, float rate);
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotState& state);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_ENV_DSP_HPP