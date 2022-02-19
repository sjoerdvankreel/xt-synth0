#ifndef XTS_UNIT_DSP_HPP
#define XTS_UNIT_DSP_HPP

#include "CvDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class UnitDSP
{
  AudioOutput _output;
  UnitModel const* _model;
  double _phase, _blepTri;
  float _rate, _pan, _amt1, _amt2, _amp, _roll, _pw, _freq;
public:
  UnitDSP() = default;
  UnitDSP(UnitModel const* model, int oct, UnitNote note, float rate);
private:
  static float Freq(UnitModel const& model, int oct, UnitNote note);
  float ModFreq(ModInput const& mod) const;
  float ModPhase(ModInput const& mod) const;
  float Generate(float phase, float freq, ModInput const& mod);
  float GenerateBlep(float phase, float freq, ModInput const& mod);
  float GenerateAdd(float phase, float freq, ModInput const& mod) const;
  float Mod(UnitModTarget tgt, float val, bool bip, ModInput const& mod) const;
public:
  void Next(CvState const& cv);
  AudioOutput const Output() const { return _output; }
  static void Plot(UnitModel const& model, CvModel const& cv, PlotInput const& input, PlotOutput& output);
};

} // namespace Xts
#endif // XTS_UNIT_DSP_HPP