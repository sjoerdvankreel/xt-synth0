#ifndef XTS_DSP_SYNTH_LFO_DSP_HPP
#define XTS_DSP_SYNTH_LFO_DSP_HPP

#include <DSP/Plot.hpp>
#include <DSP/CvSample.hpp>
#include <Model/Synth/LfoModel.hpp>

namespace Xts {

class LfoDSP
{
  double _phase;
  float _base;
  float _factor;
  float _increment;
  CvSample _output;
  struct LfoModel const* _model;
private:
  float Generate() const;
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
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotOutput& output);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_LFO_DSP_HPP