#ifndef XTS_UNIT_DSP_HPP
#define XTS_UNIT_DSP_HPP

#include "../Model/SynthModel.hpp"

namespace Xts {

class UnitDSP 
{
  float _phasef;
  double _phased;
public:  
  void Reset();
  float Next(GlobalModel const& global, UnitModel const& unit, float rate);
};

} // namespace Xts
#endif // XTS_UNIT_DSP_HPP