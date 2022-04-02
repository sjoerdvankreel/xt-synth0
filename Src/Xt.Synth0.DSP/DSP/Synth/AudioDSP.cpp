#include <DSP/Synth/CvState.hpp>
#include <DSP/Synth/AudioDSP.hpp>
#include <Model/Synth/AudioModel.hpp>

#include <cassert>

namespace Xts {

AudioState const&
AudioDSP::Next(CvState const& cv)
{
  for (int i = 0; i < XTS_VOICE_UNIT_COUNT; i++) _output.units[i] = _units[i].Next(cv);
  for (int i = 0; i < XTS_VOICE_FILTER_COUNT; i++) _output.filters[i] = _filters[i].Next(cv, Output());
  return Output();
}

AudioDSP::
AudioDSP(AudioModel const* model, int octave, NoteType note, float rate):
AudioDSP()
{
  for (int i = 0; i < XTS_VOICE_UNIT_COUNT; i++) _units[i] = UnitDSP(&model->units[i], octave, note, rate);
  for (int i = 0; i < XTS_VOICE_FILTER_COUNT; i++) _filters[i] = VoiceFilterDSP(&model->filters[i], i, rate);
}

} // namespace Xts