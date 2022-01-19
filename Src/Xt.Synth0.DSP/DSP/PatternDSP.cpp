#include "PatternDSP.hpp"
#include <cassert>

namespace Xts {

void 
PatternDSP::Automate(PatternFx const& fx, SynthModel& synth) const
{
  assert(fx.val >= 0);
  assert(fx.tgt >= 0);
  if (fx.tgt == 0 || fx.tgt > AutoParamCount) return;
  auto const& p = synth.autoParams[fx.tgt - 1];
  if (fx.val < p.min) *p.val = p.min;
  else if (fx.val > p.max) *p.val = p.max;
  else *p.val = fx.val;
}

void
PatternDSP::Automate(EditModel const& edit, PatternRow const& row, SynthModel& synth) const
{
  for (int f = 0; f < edit.fxs; f++)
    Automate(row.fx[f], synth);
}

} // namespace Xts