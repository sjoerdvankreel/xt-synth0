#ifndef XTS_LFO_DSP_HPP
#define XTS_LFO_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

struct LfoPlotState
{
  bool spectrum;
  PlotOutput* output;
  LfoModel const* model;
  PlotInput const* input;
};

class LfoDSP
{
  double _phase;
  CvSample _output;
  LfoModel const* _model;
  float _incr, _base, _factor;
public:
  LfoDSP() = default;
  LfoDSP(LfoModel const* model, float bpm, float rate);
private:
  float Generate() const;
  static float Freq(LfoModel const& model, float bpm, float rate);
public:
  CvSample Next();
  CvSample Output() const { return _output; }
  static void Plot(LfoPlotState* state);
};

} // namespace Xts
#endif // XTS_LFO_DSP_HPP