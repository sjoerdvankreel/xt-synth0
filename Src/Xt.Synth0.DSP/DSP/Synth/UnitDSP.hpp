#ifndef XTS_DSP_SYNTH_UNIT_DSP_HPP
#define XTS_DSP_SYNTH_UNIT_DSP_HPP

#include <DSP/AudioSample.hpp>
#include <DSP/Synth/ModDSP.hpp>
#include <Model/Synth/UnitModel.hpp>

namespace Xts {

struct UnitPlotState
{
  bool spectrum;
  struct CvModel const* cv;
  struct PlotOutput* output;
  struct UnitModel const* model;
  struct PlotInput const* input;
};

class UnitDSP
{
  float _amp;
  float _rate;
  float _panning;
  float _frequency;
  float _blepPulseWidth;
  float _additiveRolloff;
  ModDSP _mod1;
  ModDSP _mod2;
  double _phase;
  double _blepTriangle;
  FloatSample _output;
  UnitModel const* _model;
public:
  FloatSample Output() const;
  static void Plot(UnitPlotState* state);
  FloatSample Next(struct CvState const& cv);
public:
  UnitDSP() = default;
  UnitDSP(UnitModel const* model, int octave, UnitNote note, float rate);
private:
  float ModulatePhase(CvSample modulator1, CvSample modulator2) const;
  float ModulateFrequency(CvSample modulator1, CvSample modulator2) const;
  float Generate(float phase, float frequency, CvSample modulator1, CvSample modulator2);
  float GeneratePolyBlep(float phase, float frequency, CvSample modulator1, CvSample modulator2);
  float GenerateAdditive(float phase, float frequency, CvSample modulator1, CvSample modulator2) const;
  float Modulate(UnitModTarget target, CvSample carrier, CvSample modulator1, CvSample modulator2) const;
};

inline FloatSample
UnitDSP::Output() const
{ return _output; }

} // namespace Xts
#endif // XTS_DSP_SYNTH_UNIT_DSP_HPP