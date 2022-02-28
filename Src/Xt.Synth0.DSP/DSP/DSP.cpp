#include "DSP.hpp"
#include <DSP/Utility.hpp>

namespace Xts {

static void
Fft(std::complex<float>* x, std::complex<float>* scratch, size_t count)
{
  assert(count == NextPowerOf2(count));
  if(count < 2) return;  
  std::complex<float>* even = scratch;
  std::complex<float>* odd = scratch + count / 2;
  for(size_t i = 0; i < count / 2; i++) even[i] = x[i * 2];
  for(size_t i = 0; i < count / 2; i++) odd[i] = x[i * 2 + 1];
  Fft(odd, x, count / 2);
  Fft(even, x, count / 2);
  for (size_t i = 0; i < count / 2; i++)
  {
    float im = -2.0f * PIF * i / count;
    std::complex<float> t = std::polar(1.0f, im) * odd[i];
    x[i] = even[i] + t;
    x[i + count/2] = even[i] - t;
  }
}

void 
Fft(std::vector<float> const& x, std::vector<std::complex<float>>& fft, std::vector<std::complex<float>>& scratch)
{
  assert(x.size() == NextPowerOf2(x.size()));
  fft.resize(x.size());
  scratch.resize(x.size());
  for(size_t i = 0; i < x.size(); i++)
    fft[i] = std::complex<float>(x[i], 0.0f);
  Fft(fft.data(), scratch.data(), x.size());
}

static bool
ModBip(CvState const& cv, ModSource mod)
{
  if (mod == ModSource::LFO1 && cv.lfos[0].bipolar) return true;
  if (mod == ModSource::LFO2 && cv.lfos[1].bipolar) return true;
  if (mod == ModSource::LFO3 && cv.lfos[2].bipolar) return true;
  return false;
}

static float
ModVal(CvState const& cv, ModSource mod)
{
  int env = static_cast<int>(ModSource::Env1);
  int lfo = static_cast<int>(ModSource::LFO1);
  switch (mod)
  {
  case ModSource::Velo: 
    return cv.velocity;
  case ModSource::Env1: case ModSource::Env2: case ModSource::Env3:
    return cv.envelopes[static_cast<int>(mod) - env].value;
  case ModSource::LFO1: case ModSource::LFO2: case ModSource::LFO3:
    return cv.lfos[static_cast<int>(mod) - lfo].value;
  default: assert(false); return 0.0f;
  }
}

CvSample
ModulationInput(CvState const& cv, ModSource src)
{
  CvSample result;
  result.value = ModVal(cv, src);
  result.bipolar = ModBip(cv, src);
  return result;
}

ModInput
ModulationInput(CvState const& cv, ModSource src1, ModSource src2)
{
  ModInput result;
  result.cv1 = ModulationInput(cv, src1);
  result.cv2 = ModulationInput(cv, src2);
  return result;
}

float 
Modulate(float val, bool bip, float amt, CvSample cv)
{
  float range = 0.0f;
  val = EpsilonToZero(val);
  assert(-1.0f <= amt && amt <= 1.0f);
  assert(bip || 0.0f <= val && val <= 1.0f);
  assert(cv.bipolar || 0.0f <= cv.value && cv.value <= 1.0f);
  assert(!bip || -1.0f <= val && val <= 1.0f);
  assert(!cv.bipolar || -1.0f <= cv.value && cv.value <= 1.0f);
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