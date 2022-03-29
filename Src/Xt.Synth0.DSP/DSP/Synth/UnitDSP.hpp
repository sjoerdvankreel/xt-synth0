#ifndef XTS_DSP_SYNTH_UNIT_DSP_HPP
#define XTS_DSP_SYNTH_UNIT_DSP_HPP

#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Synth/LfoDSP.hpp>
#include <DSP/Synth/TargetModsDSP.hpp>
#include <DSP/Shared/Plot.hpp>
#include <DSP/Shared/AudioSample.hpp>
#include <Model/Synth/UnitModel.hpp>

namespace Xts {

class UnitDSP
{
  float _rate;
  int _octave;
  double _phase;
  UnitNote _note;
  TargetModsDSP _mods;
  FloatSample _output;
  double _blepTriangle;
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
  int _index;
  CvDSP _cvDsp;
  UnitDSP _unitDsp;
  LfoDSP _globalLfoDsp;
  struct SynthModel const* _model;
public:
  float Next() { return _unitDsp.Next(_cvDsp.Next(_globalLfoDsp.Next())).Mono(); }
  UnitPlot(struct SynthModel const* model, int index) : _model(model), _index(index) {}
public:
  PeriodicParams Params() const;
  void Init(float bpm, float rate);
  float Frequency(float bpm, float rate) const;
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotState& state);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_UNIT_DSP_HPP