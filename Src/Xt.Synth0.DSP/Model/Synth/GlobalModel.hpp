#ifndef XTS_MODEL_SYNTH_GLOBAL_MODEL_HPP
#define XTS_MODEL_SYNTH_GLOBAL_MODEL_HPP

#include <Model/Synth/LfoModel.hpp>
#include <Model/Synth/PlotModel.hpp>

namespace Xts {

struct XTS_ALIGN GlobalModel
{
  LfoModel lfo;
  PlotModel plot;
};
XTS_CHECK_SIZE(GlobalModel, 48);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_GLOBAL_MODEL_HPP