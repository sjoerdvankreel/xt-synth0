#include <DSP/Synth/CvState.hpp>
#include <DSP/Synth/VoiceModDSP.hpp>

namespace Xts {

static bool
ModulatorIsBipolar(VoiceModSource source, CvState const& cv)
{
  if (source == VoiceModSource::LFO1 && cv.lfos[0].bipolar) return true;
  if (source == VoiceModSource::LFO2 && cv.lfos[1].bipolar) return true;
  if (source == VoiceModSource::GlobalLFO && cv.globalLfo.bipolar) return true;
  return false;
}

static float
ModulatorValue(VoiceModSource source, CvState const& cv)
{
  int index = static_cast<int>(source);
  int env = static_cast<int>(VoiceModSource::Env1);
  int lfo = static_cast<int>(VoiceModSource::LFO1);
  switch (source)
  {
  case VoiceModSource::Velocity: return cv.velocity;
  case VoiceModSource::GlobalLFO: return cv.globalLfo.value;
  case VoiceModSource::LFO1: case VoiceModSource::LFO2: return cv.lfos[index - lfo].value;
  case VoiceModSource::Env1: case VoiceModSource::Env2: case VoiceModSource::Env3: return cv.envs[index - env].value;
  default: assert(false); return 0.0f;
  }
}

CvSample
VoiceModDSP::Next(CvState const& cv)
{
  _amount = Param::Mix(_model->amount);
  _output.value = ModulatorValue(_model->source, cv);
  _output.bipolar = ModulatorIsBipolar(_model->source, cv);
  return _output.Sanity();
}

} // namespace Xts