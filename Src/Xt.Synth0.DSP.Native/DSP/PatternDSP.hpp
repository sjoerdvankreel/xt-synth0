#ifndef XTS_PATTERN_DSP_HPP
#define XTS_PATTERN_DSP_HPP

#include "../Model/SynthModel.hpp"
#include "../Model/SequencerModel.hpp"
#include <vector>

namespace Xts {

struct Param
{
  int* value;
  int min, max;
};

class PatternDSP
{
  std::vector<Param> const _params;
  void Automate(PatternFx const& fx, SynthModel& synth) const;
public:
  PatternDSP(std::vector<Param> params): _params(params) {}
  void Automate(EditModel const& edit, PatternRow const& row, SynthModel& synth) const;
};

} // namespace Xts
#endif // XTS_PATTERN_DSP_HPP