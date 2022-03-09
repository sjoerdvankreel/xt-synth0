#ifndef XTS_MODEL_SYNTH_PLOT_MODEL_HPP
#define XTS_MODEL_SYNTH_PLOT_MODEL_HPP

#include <Model/Model.hpp>

namespace Xts {

enum class PlotType
{ 
  Synth, Amp, 
  Env1, Env2, Env3, 
  LFO1, LFO2, LFO3, 
  Unit1, Unit2, Unit3, 
  Filter1, Filter2, Filter3 
};

struct XTS_ALIGN PlotModel
{
  XtsBool on, spec;
  PlotType type;
  int32_t hold;
};
XTS_CHECK_SIZE(PlotModel, 16);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_PLOT_MODEL_HPP