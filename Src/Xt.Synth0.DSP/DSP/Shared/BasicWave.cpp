#include <DSP/Shared/BasicWave.hpp>
#include <DSP/Shared/Utility.hpp>

#include <cmath>
#include <cassert>

namespace Xts {

float
GenerateBasicWave(BasicWaveType type, double phase)
{
  float phasef = static_cast<float>(phase);
  switch (type)
  {
  case BasicWaveType::Saw: return phasef * 2.0f - 1.0f;
  case BasicWaveType::Sqr: return phasef < 0.5f ? 1.0f : -1.0f;
  case BasicWaveType::Sin: return std::sinf(phasef * 2.0f * PIF);
  case BasicWaveType::Tri: return 4.0f * (phasef < 0.25f ? phasef : phasef < 0.75f ? 0.5f - phasef : (phasef - 0.75f) - 0.25f);
  default: assert(false); return 0.0f;
  }
}

} // namespace Xts