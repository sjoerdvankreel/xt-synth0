#ifndef XTS_MODEL_SYNTH_AUDIO_MODEL_HPP
#define XTS_MODEL_SYNTH_AUDIO_MODEL_HPP

#include <Model/Model.hpp>
#include <Model/Synth/Config.hpp>
#include <Model/Synth/UnitModel.hpp>
#include <Model/Synth/FilterModel.hpp>

namespace Xts {

struct XTS_ALIGN AudioModel
{
  UnitModel units[XTS_SYNTH_UNIT_COUNT];
  FilterModel filters[XTS_SYNTH_FILTER_COUNT];
};
XTS_CHECK_SIZE(AudioModel, 552);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_AUDIO_MODEL_HPP