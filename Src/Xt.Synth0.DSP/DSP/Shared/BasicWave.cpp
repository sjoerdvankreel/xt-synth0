#include <DSP/Shared/BasicWave.hpp>
#include <DSP/Shared/Utility.hpp>

#include <cmath>
#include <cassert>

namespace Xts {

float
GenerateBasicWave(BasicWaveType type, float phase)
{
  switch (type)
  {
  case BasicWaveType::Saw: return phase * 2.0f - 1.0f;
  case BasicWaveType::Square: return phase < 0.5f ? 1.0f : -1.0f;
  case BasicWaveType::Sine: return std::sinf(phase * 2.0f * PIF);
  case BasicWaveType::Triangle: return 4.0f * (phase < 0.25f ? phase : phase < 0.75f ? 0.5f - phase : (phase - 0.75f) - 0.25f);
  default: assert(false); return 0.0f;
  }
}

} // namespace Xts