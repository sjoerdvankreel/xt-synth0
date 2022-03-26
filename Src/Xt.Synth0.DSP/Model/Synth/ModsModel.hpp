#ifndef XTS_MODEL_SYNTH_MODS_MODEL_HPP
#define XTS_MODEL_SYNTH_MODS_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Synth/ModModel.hpp>

namespace Xts {

struct XTS_ALIGN ModsModel
{
  ModModel mod1;
  ModModel mod2;
};
XTS_CHECK_SIZE(ModsModel, 32);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_MODS_MODEL_HPP