#ifndef XTS_UNIT_DSP_HPP
#define XTS_UNIT_DSP_HPP

#include "../Model/SynthModel.hpp"
#include "../Model/TrackConstants.hpp"

namespace Xts {

class UnitDSP 
{
  float _phasef;
  double _phased;

  float GenerateNaive(UnitType type);
  float GenerateAdditive(UnitType type, float freq, float rate, int logHarmonics);
  float Generate(GlobalModel const& global, UnitType type, float freq, float rate);
  float GenerateMethod(GlobalModel const& global, UnitType type, float freq, float rate);
  float GenerateAdditive(float freq, float rate, int logHarmonics, int step, int multiplier, int logRolloff);

public:
  void Reset();
  static void Init();
  float Next(GlobalModel const& global, UnitModel const& unit, float rate);
};

} // namespace Xts
#endif // XTS_UNIT_DSP_HPP