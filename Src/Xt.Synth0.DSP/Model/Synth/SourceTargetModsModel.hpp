#ifndef XTS_MODEL_SYNTH_SOURCE_TARGET_MODS_MODEL_HPP
#define XTS_MODEL_SYNTH_SOURCE_TARGET_MODS_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Synth/SourceTargetModModel.hpp>

namespace Xts {

struct XTS_ALIGN SourceTargetModsModel
{
  SourceTargetModModel mod1;
  SourceTargetModModel mod2;
};
XTS_CHECK_SIZE(SourceTargetModsModel, 32);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_SOURCE_TARGET_MODS_MODEL_HPP