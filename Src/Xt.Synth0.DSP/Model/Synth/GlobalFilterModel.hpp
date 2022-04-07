#ifndef XTS_MODEL_SYNTH_GLOBAL_FILTER_MODEL_HPP
#define XTS_MODEL_SYNTH_GLOBAL_FILTER_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Synth/FilterModel.hpp>
#include <Model/Synth/GlobalModModel.hpp>

namespace Xts {

struct XTS_ALIGN GlobalFilterModel
{
  FilterModel filter;
  GlobalModModel mod;
};
XTS_CHECK_SIZE(GlobalFilterModel, 56);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_GLOBAL_FILTER_MODEL_HPP