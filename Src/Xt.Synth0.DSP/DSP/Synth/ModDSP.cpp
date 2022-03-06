#include <DSP/Synth/ModDSP.hpp>
#include <DSP/Synth/CvState.hpp>

namespace Xts {

static bool
ModulatorIsBipolar(ModSource source, CvState const& cv)
{
  if (source == ModSource::LFO1 && cv.lfos[0].bipolar) return true;
  if (source == ModSource::LFO2 && cv.lfos[1].bipolar) return true;
  if (source == ModSource::LFO3 && cv.lfos[2].bipolar) return true;
  return false;
}

static float
ModulatorValue(ModSource source, CvState const& cv)
{
  int index = static_cast<int>(source);
  int env = static_cast<int>(ModSource::Env1);
  int lfo = static_cast<int>(ModSource::LFO1);
  switch (source)
  {
  case ModSource::Velocity: return cv.velocity;
  case ModSource::Env1: case ModSource::Env2: case ModSource::Env3: return cv.envs[index - env].value;
  case ModSource::LFO1: case ModSource::LFO2: case ModSource::LFO3: return cv.lfos[index - lfo].value;
  default: assert(false); return 0.0f;
  }
}

CvSample
ModDSP::Modulator(CvState const& cv) const
{
  CvSample result;
  result.value = ModulatorValue(_source, cv);
  result.bipolar = ModulatorIsBipolar(_source, cv);
  return result.Sanity();
}

float
ModDSP::Modulate(CvSample carrier, CvSample modulator) const
{
  float range = 0.0f;
  if(_amount == 0.0f) return carrier.value;
  if (!modulator.bipolar && _amount > 0.0f) range = 1.0f - carrier.value;
  if (!modulator.bipolar && !carrier.bipolar && _amount < 0.0f) range = carrier.value;
  if (!modulator.bipolar && carrier.bipolar && _amount < 0.0f) range = 1.0f + carrier.value;
  if (modulator.bipolar && carrier.bipolar) range = 1.0f - std::fabs(carrier.value);
  if (modulator.bipolar && !carrier.bipolar) range = 0.5f - std::fabs(carrier.value - 0.5f);
  float result = carrier.value + modulator.value * _amount * range;
  if(carrier.bipolar) BipolarSanity(result);
  else UnipolarSanity(result);
  return result;
}

} // namespace Xts