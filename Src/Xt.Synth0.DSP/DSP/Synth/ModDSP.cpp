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

static CvSample
ModulatorOutput(ModSource source, CvState const& cv)
{
  CvSample result;
  result.value = ModulatorValue(source, cv);
  result.bipolar = ModulatorIsBipolar(source, cv);
  return result.Sanity();
}

float
ModDSP::Modulate(CvSample sample, CvState const& cv)
{
  if(_amount == 0.0f) return sample.value;

}



float
Modulate(float val, bool bip, float amt, CvSample cv)
{
  float range = 0.0f;
  if (amt == 0.0f) return val;
  if (!cv.bipolar && amt > 0.0f) range = 1.0f - val;
  if (!cv.bipolar && !bip && amt < 0.0f) range = val;
  if (!cv.bipolar && bip && amt < 0.0f) range = 1.0f + val;
  if (cv.bipolar && bip) range = 1.0f - std::fabs(val);
  if (cv.bipolar && !bip) range = 0.5f - std::fabs(val - 0.5f);
  float result = val + cv.value * amt * range;
  assert(bip || 0.0f <= result && result <= 1.0f);
  assert(!bip || -1.0f <= result && result <= 1.0f);
  return result;
}

} // namespace Xts