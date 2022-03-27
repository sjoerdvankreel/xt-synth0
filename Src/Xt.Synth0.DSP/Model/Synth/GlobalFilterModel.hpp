#ifndef XTS_MODEL_SYNTH_GLOBAL_FILTER_MODEL_HPP
#define XTS_MODEL_SYNTH_GLOBAL_FILTER_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Synth/FilterModel.hpp>

namespace Xts {

struct XTS_ALIGN GlobalFilterModel
{
  FilterModel filter;
  int32_t lfoAmount;
  FilterModTarget lfoTarget;
};
XTS_CHECK_SIZE(GlobalFilterModel, 48);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_GLOBAL_FILTER_MODEL_HPP