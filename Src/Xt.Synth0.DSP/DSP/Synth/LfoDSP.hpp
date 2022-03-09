#ifndef XTS_DSP_SYNTH_LFO_DSP_HPP
#define XTS_DSP_SYNTH_LFO_DSP_HPP

#include <DSP/CvSample.hpp>

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
  CvSample Output() const;
  static void Plot(struct SynthModel const& model, struct PlotInput const& input, struct PlotOutput& output);
};

inline CvSample 
LfoDSP::Output() const 
{ return _output; }

} // namespace Xts
#endif // XTS_DSP_SYNTH_LFO_DSP_HPP