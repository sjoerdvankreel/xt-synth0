#include "AudioDSP.hpp"

namespace Xts {

AudioState const&
AudioDSP::Next(CvState const& cv)
{
  for (int i = 0; i < UnitCount; i++) 
    _output.units[i] = _units[i].Next(cv);
  for (int i = 0; i < FilterCount; i++)
    _output.filts[i].Clear();
  for(int i = 0; i < FilterCount; i++)
    _output.filts[i] = _flts[i].Next(cv, Output());
  return Output();
}

AudioDSP::
AudioDSP(AudioModel const* model, int oct, UnitNote note, float rate):
_output(), _units(), _flts()
{
  for (int i = 0; i < FilterCount; i++)
    _flts[i] = FilterDSP(&model->filts[i], rate);
  for (int i = 0; i < UnitCount; i++)
    _units[i] = UnitDSP(&model->units[i], oct, note, rate);
}

} // namespace Xts