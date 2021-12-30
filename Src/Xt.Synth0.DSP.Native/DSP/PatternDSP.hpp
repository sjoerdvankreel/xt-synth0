#ifndef XTS_PATTERN_DSP_HPP
#define XTS_PATTERN_DSP_HPP

#include "../Model/SynthModel.hpp"
#include "../Model/SequencerModel.hpp"
#include <vector>

namespace Xts {

class PatternDSP
{
  void Automate(
    PatternFx const& fx,
    std::vector<int*> const& params,
    SynthModel& synth) const;

public:
  void Automate(
    EditModel const& edit,
    PatternRow const& row,
    std::vector<int*> const& params,
    SynthModel& synth) const;
};

} // namespace Xts
#endif // XTS_PATTERN_DSP_HPP