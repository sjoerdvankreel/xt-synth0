#ifndef XTS_UNIT_DSP_HPP
#define XTS_UNIT_DSP_HPP

#include "../Model/SynthModel.hpp"
#include <vector>

namespace Xts {

class UnitDSP 
{
  static float FrequencyTable;

  float _phasef;
  double _phased;
public:  
  void Reset();
  static void Init();
  float Next(GlobalModel const& global, UnitModel const& unit, float rate);
};

} // namespace Xts
#endif // XTS_UNIT_DSP_HPP