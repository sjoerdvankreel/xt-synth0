#ifndef XTS_LFO_DSP_HPP
#define XTS_LFO_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class LfoDSP
{
  double _phase;
  CVOutput _output;
  LfoModel const* _model;
  float _incr, _base, _factor;
private:
  float Generate() const;
  static float Freq(LfoModel const& model, float bpm, float rate);
public:
  LfoDSP() = default;
  LfoDSP(LfoModel const* model, float bpm, float rate);
public:
  void Next();
  CVOutput Output() const { return _output; }
  static void Plot(LfoModel const& model, PlotInput const& input, PlotOutput& output);
};

} // namespace Xts
#endif // XTS_LFO_DSP_HPP