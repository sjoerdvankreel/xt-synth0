#include "PatternDSP.hpp"

namespace Xts {

void
PatternDSP::Automate(EditModel const& edit, PatternRow const& row, SynthModel& synth) const
{
  for(int f = 0; f < edit.fx; f++)
    Automate(row.fx[f], synth);
}

void 
PatternDSP::Automate(PatternFx const& fx, SynthModel& synth) const
{
		if (fx.target == 0 || fx.target > TrackConstants::ParamCount) return;
    auto const& p = synth.params[fx.target - 1];
		if (fx.value < p.min) *p.value = p.min;
		else if (fx.value > p.max) *p.value = p.max;
		else *p.value = fx.value;
}

} // namespace Xts