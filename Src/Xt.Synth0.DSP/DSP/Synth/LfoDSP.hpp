#ifndef XTS_DSP_SYNTH_LFO_DSP_HPP
#define XTS_DSP_SYNTH_LFO_DSP_HPP

#include <DSP/Shared/Plot.hpp>
#include <DSP/Shared/Prng.hpp>
#include <DSP/Shared/CvSample.hpp>
#include <Model/Synth/LfoModel.hpp>

namespace Xts {

class LfoDSP
{
  Prng _prng;
  float _bpm;
  float _rate;
  double _phase;
  float _randDir;
  int _randCount;
  float _filtered;
  float _randLevel;
  float _randState;
  CvSample _output;
  struct LfoModel const* _model;
private:
  void InitRandom();
  float GenerateWave() const;
  float Generate(float period);
  int NextRandomCount(float period);
  float GenerateRandom(float period);
  float Filter(float period, float x);
  float NextRandomState(float steepness);
  float NextRandomLevel(float steepness, int count);
public:
  LfoDSP() = default;
  LfoDSP(LfoModel const* model, float bpm, float rate);
public:
  CvSample Next();
  CvSample Output() const { return _output; }
  static float Frequency(LfoModel const& model, float bpm, float rate);
};

class LfoPlot : 
public PeriodicPlot
{
  LfoDSP _dsp;
  LfoModel const* _model;
public:
  LfoPlot(LfoModel const* model) : _model(model) {}
public:
  float Next() { return _dsp.Next().value; }
  void Init(float bpm, float rate) { new(&_dsp) LfoDSP(_model, bpm, rate); }
  float Frequency(float bpm, float rate) const { return LfoDSP::Frequency(*_model, bpm, rate); }
public:
  PeriodicParams Params() const;
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotState& state);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_LFO_DSP_HPP