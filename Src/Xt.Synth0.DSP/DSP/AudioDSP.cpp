#include "AudioDSP.hpp"

namespace Xts {

AudioState const&
AudioDSP::Next(CvState const& cv)
{
  for (int i = 0; i < UnitCount; i++) 
    _output.units[i] = _units[i].Next(cv);
  return Output();
}

AudioDSP::
AudioDSP(AudioModel const* model, int oct, UnitNote note, float rate):
_output(), _units()
{
  for (int i = 0; i < UnitCount; i++) 
    _units[i] = UnitDSP(&model->units[i], oct, note, rate);
}

} // namespace Xts