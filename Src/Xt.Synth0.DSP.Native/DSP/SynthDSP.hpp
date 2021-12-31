#ifndef XTS_SYNTH_DSP_HPP
#define XTS_SYNTH_DSP_HPP

#include "UnitDSP.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class SynthDSP
{
  UnitDSP _units[SynthModel::UnitCount];
public:
  void Reset();
  float Next(SynthModel const& synth, float rate) const;
};

} // namespace Xts
#endif // XTS_SYNTH_DSP_HPP