#ifndef XTS_MODEL_SYNTH_GLOBAL_MODEL_HPP
#define XTS_MODEL_SYNTH_GLOBAL_MODEL_HPP

#include <Model/Synth/LfoModel.hpp>
#include <Model/Synth/PlotModel.hpp>
#include <Model/Synth/GlobalFilterModel.hpp>

namespace Xts {

struct XTS_ALIGN GlobalModel
{
  LfoModel lfo;
  PlotModel plot;
  GlobalFilterModel filter;
};
XTS_CHECK_SIZE(GlobalModel, 96);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_GLOBAL_MODEL_HPP