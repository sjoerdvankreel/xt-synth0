#ifndef XTS_MODEL_SYNTH_AUDIO_MODEL_HPP
#define XTS_MODEL_SYNTH_AUDIO_MODEL_HPP

#include <Model/Model.hpp>
#include <Model/Synth/UnitModel.hpp>
#include <Model/Synth/FilterModel.hpp>

namespace Xts {

struct XTS_ALIGN AudioModel
{
  UnitModel units[UnitCount];
  FilterModel filts[FilterCount];
};
XTS_CHECK_SIZE(AudioModel, 528);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_AUDIO_MODEL_HPP