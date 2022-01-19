#include "PatternDSP.hpp"
#include <cassert>

namespace Xts {

void 
PatternDSP::Automate(PatternFx const& fx, SynthModel& synth) const
{
  assert(fx.value >= 0);
  assert(fx.target >= 0);
  if (fx.target == 0 || fx.target > AutoParamCount) return;
  auto const& p = synth.autoParams[fx.target - 1];
  if (fx.value < p.min) *p.val = p.min;
  else if (fx.value > p.max) *p.val = p.max;
  else *p.val = fx.value;
}

void
PatternDSP::Automate(EditModel const& edit, PatternRow const& row, SynthModel& synth) const
{
  for (int f = 0; f < edit.fxs; f++)
    Automate(row.fx[f], synth);
}

} // namespace Xts