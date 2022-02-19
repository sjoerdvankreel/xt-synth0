#include "AudioDSP.hpp"

namespace Xts {

void
AudioDSP::Next(CvState const& cv)
{
  for (int u = 0; u < UnitCount; u++)
  {
    _units[u].Next(cv);
    _output.units[u] = _units[u].Output();
  }
}

AudioDSP::
AudioDSP(AudioModel const* model, int oct, UnitNote note, float rate):
_output(), _units()
{
  for (int u = 0; u < UnitCount; u++)
    _units[u] = UnitDSP(&model->units[u], oct, note, rate);
}

} // namespace Xts