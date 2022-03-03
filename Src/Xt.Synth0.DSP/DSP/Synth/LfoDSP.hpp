#ifndef XTS_DSP_SYNTH_LFO_DSP_HPP
#define XTS_DSP_SYNTH_LFO_DSP_HPP

#include <DSP/Synth/CvSample.hpp>
#include <Model/Synth/LfoModel.hpp>

namespace Xts {

struct LfoPlotState
{
  bool spectrum;
  LfoModel const* model;
  struct PlotOutput* output;
  struct PlotInput const* input;
};

class LfoDSP
{
  double _phase;
  float _base;
  float _factor;
  float _increment;
  CvSample _output;
  LfoModel const* _model;
public:
  CvSample Next();
  CvSample Output() const;
  static void Plot(LfoPlotState* state);
public:
  LfoDSP() = default;
  LfoDSP(LfoModel const* model, float bpm, float rate);
private:
  float Generate() const;
  static float Frequency(LfoModel const& model, float bpm, float rate);
};

inline CvSample
LfoDSP::Output() const
{ return _output; }

} // namespace Xts
#endif // XTS_DSP_SYNTH_LFO_DSP_HPP