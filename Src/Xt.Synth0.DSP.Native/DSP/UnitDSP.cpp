#include "UnitDSP.hpp"

namespace Xts {

void
UnitDSP::Init()
{
}

void
UnitDSP::Reset()
{
  _phased = 0.0;
  _phasef = 0.0f;
}

float
UnitDSP::Next(GlobalModel const& global, UnitModel const& unit, float rate)
{
  return 0.0f;
}

} // namespace Xts