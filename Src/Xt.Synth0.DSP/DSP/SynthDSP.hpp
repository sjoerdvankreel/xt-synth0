#ifndef XTS_SYNTH_DSP_HPP
#define XTS_SYNTH_DSP_HPP

#include "UnitDSP.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class SynthDSP
{
  UnitDSP _units[TrackConstants::UnitCount];
public:
  void Reset();
  void Next(SynthModel const& synth, float rate, float* l, float* r);
};

} // namespace Xts
#endif // XTS_SYNTH_DSP_HPP