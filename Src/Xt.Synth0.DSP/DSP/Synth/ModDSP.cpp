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
  double range = 0.0f;
  if(_amount == 0.0f) return carrier.value;
  double carrierValue = EpsilonToZero(carrier.value);
  double modulatorValue = EpsilonToZero(modulator.value);
  if (!modulator.bipolar && _amount > 0.0f) range = 1.0 - carrierValue;
  if (!modulator.bipolar && !carrier.bipolar && _amount < 0.0f) range = carrierValue;
  if (!modulator.bipolar && carrier.bipolar && _amount < 0.0f) range = 1.0 + carrierValue;
  if (modulator.bipolar && carrier.bipolar) range = 1.0 - std::fabs(carrierValue);
  if (modulator.bipolar && !carrier.bipolar) range = 0.5 - std::fabs(carrierValue - 0.5f);
  double result = carrierValue + modulatorValue * _amount * range;
  if(carrier.bipolar) BipolarSanity(result);
  else UnipolarSanity(result);
  return static_cast<float>(result);
}

} // namespace Xts