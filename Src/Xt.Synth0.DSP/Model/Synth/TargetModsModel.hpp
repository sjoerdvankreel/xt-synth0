#ifndef XTS_MODEL_SYNTH_TARGET_MODS_MODEL_HPP
#define XTS_MODEL_SYNTH_TARGET_MODS_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Synth/TargetModModel.hpp>

namespace Xts {

struct XTS_ALIGN TargetModsModel
{
  TargetModModel mod1;
  TargetModModel mod2;
};
XTS_CHECK_SIZE(TargetModsModel, 32);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_TARGET_MODS_MODEL_HPP