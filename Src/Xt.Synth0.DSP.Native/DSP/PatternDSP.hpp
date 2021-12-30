#ifndef XTS_PATTERN_DSP_HPP
#define XTS_PATTERN_DSP_HPP

#include "../Model/SynthModel.hpp"
#include "../Model/SequencerModel.hpp"

namespace Xts {

class PatternDSP
{
public:
  void Automate(SequencerModel const& seq, SynthModel& synth) const;
};

} // namespace Xts
#endif // XTS_PATTERN_DSP_HPP