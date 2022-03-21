#ifndef XTS_DSP_SYNTH_UNIT_DSP_HPP
#define XTS_DSP_SYNTH_UNIT_DSP_HPP

#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Synth/ModsDSP.hpp>
#include <DSP/Shared/Plot.hpp>
#include <DSP/Shared/AudioSample.hpp>
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
  ModsDSP _mods;
  double _phase;
  double _blepTriangle;
  FloatSample _output;
  struct UnitModel const* _model;
private:
  float ModulatePhase() const;
  float ModulateFrequency() const;
  float Generate(float phase, float frequency);
  float GeneratePolyBlep(float phase, float frequency);
  float GenerateAdditive(float phase, float frequency) const;
public:
  FloatSample Next(struct CvState const& cv);
  FloatSample Output() const { return _output; };
  static float Frequency(UnitModel const& model, int octave, UnitNote note);
public:
  UnitDSP() = default;
  UnitDSP(struct UnitModel const* model, int octave, UnitNote note, float rate);
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
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotState& state);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_UNIT_DSP_HPP