#include "AudioDSP.hpp"
#include <Model/Synth/Config.hpp>
#include <cassert>

namespace Xts {

AudioState const&
AudioDSP::Next(CvState const& cv)
{
  for (int i = 0; i < XTS_SYNTH_UNIT_COUNT; i++) 
    _output.units[i] = _units[i].Next(cv);
  for(int i = 0; i < XTS_SYNTH_FILTER_COUNT; i++)
    _output.filters[i] = _flts[i].Next(cv, Output());
  return Output();
}

AudioDSP::
AudioDSP(AudioModel const* model, int oct, UnitNote note, float rate):
_output(), _units(), _flts()
{
  for (int i = 0; i < XTS_SYNTH_FILTER_COUNT; i++)
    _flts[i] = FilterDSP(&model->filters[i], i, rate);
  for (int i = 0; i < XTS_SYNTH_UNIT_COUNT; i++)
    _units[i] = UnitDSP(&model->units[i], oct, note, rate);
}

} // namespace Xts