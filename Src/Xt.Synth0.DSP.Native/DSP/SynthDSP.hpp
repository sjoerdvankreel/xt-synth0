#ifndef XTS_SYNTH_DSP_HPP
#define XTS_SYNTH_DSP_HPP

#include "UnitDSP.hpp"
#include "../Model/SynthModel.hpp"
#include <vector>

namespace Xts {

class SynthDSP
{
  std::vector<int*> _params;
  UnitDSP _units[SynthModel::UnitCount];
public:
  void Reset();
  int* const* Params() const { return _params.data(); }
  float Next(SynthModel const& synth, float rate) const;
  SynthDSP(std::vector<int*> const& params): _params(params), _units() {}
};

} // namespace Xts
#endif // XTS_SYNTH_DSP_HPP