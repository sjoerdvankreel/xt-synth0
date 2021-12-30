#include "PatternDSP.hpp"

namespace Xts {

void
PatternDSP::Automate(
  EditModel const& edit, 
  PatternRow const& row, 
  std::vector<int*> const& params, 
  SynthModel& synth) const
{
  for(int f = 0; f < edit.fx; f++)
    Automate(row.fx[f], params, synth);
}

void 
PatternDSP::Automate(
  PatternFx const& fx,
  std::vector<int*> const& params,
  SynthModel& synth) const
{
		if (fx.target == 0 || fx.target > params.size()) return;
		var param = autos[target - 1].Param;
		var value = fx.Value.Value;
		if (value < param.Info.Min) param.Value = param.Info.Min;
		else if (value > param.Info.Max) param.Value = param.Info.Max;
		else param.Value = value;
}

} // namespace Xts