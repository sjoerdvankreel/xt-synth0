#include "PatternDSP.hpp"
#include <cassert>

namespace Xts {

void 
PatternDSP::Automate(PatternFx const& fx, SynthModel& synth) const
{
  assert(fx.value >= 0);
  assert(fx.target >= 0);
  if (fx.target == 0 || fx.target > TrackConstants::ParamCount) return;
  auto const& p = synth.params[fx.target - 1];
  if (fx.value < p.min) *p.value = p.min;
  else if (fx.value > p.max) *p.value = p.max;
  else *p.value = fx.value;
}

void
PatternDSP::Automate(EditModel const& edit, PatternRow const& row, SynthModel& synth) const
{
  for (int f = 0; f < edit.fxs; f++)
    Automate(row.fx[f], synth);
}

} // namespace Xts