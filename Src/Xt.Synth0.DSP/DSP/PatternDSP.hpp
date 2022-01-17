#ifndef XTS_PATTERN_DSP_HPP
#define XTS_PATTERN_DSP_HPP

#include "../Model/SeqModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class PatternDSP
{
  void Automate(PatternFx const& fx, SynthModel& synth) const;

public:
  void Automate(EditModel const& edit, PatternRow const& row, SynthModel& synth) const;
};

} // namespace Xts
#endif // XTS_PATTERN_DSP_HPP