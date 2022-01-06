#ifndef XTS_UNIT_DSP_HPP
#define XTS_UNIT_DSP_HPP

#include "../Model/SynthModel.hpp"
#include "../Model/TrackConstants.hpp"

namespace Xts {

class UnitDSP 
{
  float _phasef = 0.0f;
  double _phased = 0.0;

public:
  void Reset();
  static void Init();
  float Frequency(UnitModel const& unit) const;
  float Next(UnitModel const& unit, float rate);

private:
  float GenerateNaive(UnitWave wave);
  float Generate(UnitModel const& unit, float freq, float rate);
  float GenerateType(UnitModel const& unit, float freq, float rate);
  float GenerateAdditive(UnitModel const& unit, float freq, float rate);
  float GenerateAdditive(float freq, float rate, int logHarmonics, int step, bool tri);
};

} // namespace Xts
#endif // XTS_UNIT_DSP_HPP