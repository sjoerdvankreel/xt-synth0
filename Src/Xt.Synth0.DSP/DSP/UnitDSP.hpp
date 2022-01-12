#ifndef XTS_UNIT_DSP_HPP
#define XTS_UNIT_DSP_HPP

#include "../Model/SynthModel.hpp"
#include "../Model/TrackConstants.hpp"

namespace Xts {

class UnitDSP 
{
  double _phase = 0.0;

public:
  void Reset();
  static void Init();
  float Frequency(UnitModel const& unit) const;
  float Next(UnitModel const& unit, float rate, bool* cycled);

private:
  float PwmPhase(float phase, int pwm) const;
  float GenerateNaive(NaiveType type, int pwm, float phase) const;
  float Generate(UnitModel const& unit, float freq, float rate, float phase) const;
  float GenerateAdd(UnitModel const& unit, float freq, float rate, float phase) const;
  float GenerateAdd(float freq, float rate, float phase, bool addSub, bool sinCos, int parts, int step, float logRoll) const;
};

} // namespace Xts
#endif // XTS_UNIT_DSP_HPP