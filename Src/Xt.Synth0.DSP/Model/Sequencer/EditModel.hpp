#ifndef XTS_MODEL_SEQUENCER_EDIT_MODEL_HPP
#define XTS_MODEL_SEQUENCER_EDIT_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <cstdint>

namespace Xts {

struct XTS_ALIGN EditModel
{
  int32_t fxs;
  int32_t keys;
  int32_t rows;
  int32_t patterns;

  int32_t bpm;
  int32_t lpb;

  int32_t step;
  int32_t edit;
  int32_t octave;
  XtsBool loop;
};
XTS_CHECK_SIZE(EditModel, 40);

} // namespace Xts
#endif // XTS_MODEL_SEQUENCER_EDIT_MODEL_HPP