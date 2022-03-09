#ifndef XTS_DSP_SYNTH_UNIT_DSP_HPP
#define XTS_DSP_SYNTH_UNIT_DSP_HPP

#include <DSP/Plot.hpp>
#include <DSP/AudioSample.hpp>
#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Synth/ModDSP.hpp>
#include <Model/Synth/UnitModel.hpp>

namespace Xts {

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
  struct UnitModel const* _model;
public:
  FloatSample Next(struct CvState const& cv);
  FloatSample Output() const { return _output; };
  static float Frequency(UnitModel const& model, int octave, UnitNote note);
public:
  UnitDSP() = default;
  UnitDSP(struct UnitModel const* model, int octave, UnitNote note, float rate);
private:
  float ModulatePhase(CvSample modulator1, CvSample modulator2) const;
  float ModulateFrequency(CvSample modulator1, CvSample modulator2) const;
  float Generate(float phase, float frequency, CvSample modulator1, CvSample modulator2);
  float GeneratePolyBlep(float phase, float frequency, CvSample modulator1, CvSample modulator2);
  float GenerateAdditive(float phase, float frequency, CvSample modulator1, CvSample modulator2) const;
  float Modulate(UnitModTarget target, CvSample carrier, CvSample modulator1, CvSample modulator2) const;
};

class UnitPlot : 
public PeriodicPlot
{
  CvDSP _cvDsp;
  UnitDSP _unitDsp;
  struct CvModel const* _cv;
  struct UnitModel const* _unit;
public:
  UnitPlot(struct CvModel const* cv, struct UnitModel const* unit) : _cv(cv), _unit(unit) {}
public:
  float Next() { return _unitDsp.Next(_cvDsp.Next()).Mono(); }
  float Frequency(float bpm, float rate) const { return UnitDSP::Frequency(*_unit, 4, UnitNote::C); }
public:
  PeriodicParams Params() const;
  void Init(float bpm, float rate);
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotOutput& output);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_UNIT_DSP_HPP