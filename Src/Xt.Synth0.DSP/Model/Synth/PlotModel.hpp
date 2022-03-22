#ifndef XTS_MODEL_SYNTH_PLOT_MODEL_HPP
#define XTS_MODEL_SYNTH_PLOT_MODEL_HPP

#include <Model/Shared/Model.hpp>

namespace Xts {

enum class PlotType
{ 
  Synth, Amp, 
  Env1, Env2, Env3, 
  LFO1, LFO2, GlobalLFO,
  Unit1, Unit2, Unit3, 
  Filter1, Filter2, Filter3 
};

struct XTS_ALIGN PlotModel
{
  XtsBool on;
  int32_t hold;
  PlotType type;
  XtsBool spectrum;
};
XTS_CHECK_SIZE(PlotModel, 16);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_PLOT_MODEL_HPP