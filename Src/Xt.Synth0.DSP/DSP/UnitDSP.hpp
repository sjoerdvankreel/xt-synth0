#ifndef XTS_UNIT_DSP_HPP
#define XTS_UNIT_DSP_HPP

#include "../Model/SynthModel.hpp"
#include "../Model/TrackConstants.hpp"

namespace Xts {

struct UnitOutput
{
  float l;
  float r;
  bool cycled;
  UnitOutput() = default;
};

struct AddParams
{
  int step;
  int parts;
  bool addSub;
  bool sinCos;
  float logRoll;

  AddParams() = default;
  AddParams(int p, int s, bool as, bool sc, float r):
  step(s), parts(p), addSub(as), sinCos(sc), logRoll(r) {}
};

class UnitDSP 
{
  int _noteOffset = 0;
  double _phase = 0.0;

public:
  static void Init();
  void Init(int oct, UnitNote note);
  float Freq(UnitModel const& unit) const;
  void Next(UnitModel const& unit, float rate, UnitOutput& output);

private:
  float PwPhase(int pw) const;
  float GenerateNaive(NaiveType type, int pw, float phase) const;
  float Generate(UnitModel const& unit, float freq, float rate) const;
  float GenerateAdd(UnitModel const& unit, float freq, float rate) const;
  float GenerateAdd(float freq, float rate, float phase, AddParams const& params) const;
};

} // namespace Xts
#endif // XTS_UNIT_DSP_HPP