#include <DSP/Synth/ModDSP.hpp>
#include <DSP/Synth/CvState.hpp>

namespace Xts {

static bool
ModulatorIsBipolar(ModSource source, CvState const& cv)
{
  if (source == ModSource::LFO1 && cv.lfos[0].bipolar) return true;
  if (source == ModSource::LFO2 && cv.lfos[1].bipolar) return true;
  if (source == ModSource::GlobalLFO && cv.globalLfo.bipolar) return true;
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
  case ModSource::GlobalLFO: return cv.globalLfo.value;
  case ModSource::LFO1: case ModSource::LFO2: return cv.lfos[index - lfo].value;
  case ModSource::Env1: case ModSource::Env2: case ModSource::Env3: return cv.envs[index - env].value;
  default: assert(false); return 0.0f;
  }
}

CvSample
ModDSP::Next(CvState const& cv)
{
  _amount = Param::Mix(_model->amount);
  _output.value = ModulatorValue(_model->source, cv);
  _output.bipolar = ModulatorIsBipolar(_model->source, cv);
  return _output.Sanity();
}

} // namespace Xts