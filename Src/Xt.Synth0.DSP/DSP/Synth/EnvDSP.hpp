#ifndef XTS_DSP_SYNTH_ENV_DSP_HPP
#define XTS_DSP_SYNTH_ENV_DSP_HPP

#include <DSP/EnvSample.hpp>
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
  struct EnvModel const* _model;
public:
  EnvDSP() = default;
  EnvDSP(struct EnvModel const* model, float bpm, float rate);
private:
  float Generate();
  void CycleStage(EnvType type);
  void NextStage(EnvStage stage);
  float Generate(float from, float to, SlopeType type);
  static EnvParams Params(struct EnvModel const& model, float bpm, float rate);
public:
  bool End() const;
  EnvSample Next();
  EnvSample Release();
  EnvSample Output() const;
  static void Plot(struct SynthModel const& model, struct PlotInput const& input, struct PlotOutput& output);
};

inline bool
EnvDSP::End() const
{ return _output.stage == EnvStage::End; }

} // namespace Xts
#endif // XTS_DSP_SYNTH_ENV_DSP_HPP