#ifndef XTS_UNIT_DSP_HPP
#define XTS_UNIT_DSP_HPP

#include "../Model/SynthModel.hpp"
#include "../Model/TrackConstants.hpp"

namespace Xts {

class UnitDSP 
{
  float _phasef = 0.0f;
  double _phased = 0.0;

  float GenerateNaive(UnitWave wave);
  float GenerateAdditive(UnitWave wave, float freq, float rate, int logHarmonics);
  float Generate(GlobalModel const& global, UnitWave wave, float freq, float rate);
  float GenerateAdditive(float freq, float rate, int logHarmonics, int step, bool tri);
  float GenerateMethod(GlobalModel const& global, UnitWave wave, float freq, float rate);

public:
  void Reset();
  static void Init();
  float Frequency(UnitModel const& unit) const;
  float Next(GlobalModel const& global, UnitModel const& unit, float rate);
};

} // namespace Xts
#endif // XTS_UNIT_DSP_HPP