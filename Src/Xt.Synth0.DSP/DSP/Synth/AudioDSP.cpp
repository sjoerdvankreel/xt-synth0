#include <DSP/Synth/AudioDSP.hpp>
#include <DSP/Synth/CvState.hpp>
#include <Model/Synth/AudioModel.hpp>

#include <cassert>

namespace Xts {

AudioState const&
AudioDSP::Next(CvState const& cv)
{
  for (int i = 0; i < XTS_SYNTH_UNIT_COUNT; i++) _output.units[i] = _units[i].Next(cv);
  for(int i = 0; i < XTS_SYNTH_FILTER_COUNT; i++) _output.filters[i] = _filters[i].Next(cv, Output());
  return Output();
}

AudioDSP::
AudioDSP(AudioModel const* model, int octave, UnitNote note, float rate):
AudioDSP()
{
  for (int i = 0; i < XTS_SYNTH_FILTER_COUNT; i++) _filters[i] = FilterDSP(&model->filters[i], i, rate);
  for (int i = 0; i < XTS_SYNTH_UNIT_COUNT; i++) _units[i] = UnitDSP(&model->units[i], octave, note, rate);
}

} // namespace Xts