#ifndef XTS_MODEL_SYNTH_MASTER_MODEL_HPP
#define XTS_MODEL_SYNTH_MASTER_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Synth/GlobalModModel.hpp>

namespace Xts {

struct XTS_ALIGN MasterModel
{
  int32_t amp;
  int32_t ampLfo;
  int32_t pan;
  int32_t panLfo;
};
XTS_CHECK_SIZE(MasterModel, 16);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_MASTER_MODEL_HPP