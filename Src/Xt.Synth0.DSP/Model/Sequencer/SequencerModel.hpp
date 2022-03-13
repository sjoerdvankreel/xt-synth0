#ifndef XTS_MODEL_SEQUENCER_SEQUENCER_MODEL_HPP
#define XTS_MODEL_SEQUENCER_SEQUENCER_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Sequencer/EditModel.hpp>
#include <Model/Sequencer/PatternModel.hpp>
#include <Model/Sequencer/SequencerConfig.hpp>

#include <cstdint>

namespace Xts {

struct XTS_ALIGN SequencerModel
{
  EditModel edit;
  PatternModel pattern;
};
XTS_CHECK_SIZE(SequencerModel, 22568);

} // namespace Xts
#endif // XTS_MODEL_SEQUENCER_SEQUENCER_MODEL_HPP