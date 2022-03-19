#ifndef XTS_MODEL_SYNTH_MODS_MODEL_HPP
#define XTS_MODEL_SYNTH_MODS_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Synth/ModModel.hpp>

namespace Xts {

template <class Target>
struct XTS_ALIGN ModsModel
{
  ModModel<Target> mod1;
  ModModel<Target> mod2;
};
typedef ModsModel<int32_t> AnyModsModel;
XTS_CHECK_SIZE(AnyModsModel, 32);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_MODS_MODEL_HPP