#include "DSP.hpp"
#include "SourceDSP.hpp"

namespace Xts {

static void
Fft(std::complex<float>* x, std::complex<float>* scratch, size_t count)
{
  assert(count == NextPow2(count));
  if(count < 2) return;  
  std::complex<float>* even = scratch;
  std::complex<float>* odd = scratch + count / 2;
  for(size_t i = 0; i < count / 2; i++) even[i] = x[i * 2];
  for(size_t i = 0; i < count / 2; i++) odd[i] = x[i * 2 + 1];
  Fft(odd, x, count / 2);
  Fft(even, x, count / 2);
  for (size_t i = 0; i < count / 2; i++)
  {
    float im = -2.0f * PI * i / count;
    std::complex<float> t = std::polar(1.0f, im) * odd[i];
    x[i] = even[i] + t;
    x[i + count/2] = even[i] - t;
  }
}

void 
Fft(std::vector<float> const& x, std::vector<std::complex<float>>& fft, std::vector<std::complex<float>>& scratch)
{
  assert(x.size() == NextPow2(x.size()));
  fft.resize(x.size());
  scratch.resize(x.size());
  for(size_t i = 0; i < x.size(); i++)
    fft[i] = std::complex<float>(x[i], 0.0f);
  Fft(fft.data(), scratch.data(), x.size());
}

static float
ModVal(SourceDSP const& source, ModSource mod)
{
  int env = static_cast<int>(ModSource::Env1);
  int lfo = static_cast<int>(ModSource::LFO1);
  switch (mod)
  {
  case ModSource::Velo: return source.Velo();
  case ModSource::LFO1: case ModSource::LFO2: case ModSource::LFO3:
    return source.Lfos()[static_cast<int>(mod) - lfo].Value();
  case ModSource::Env1: case ModSource::Env2: case ModSource::Env3:
    return source.Envs()[static_cast<int>(mod) - env].Value();
  default: assert(false); return 0.0f;
  }
}

static bool
ModBip(SourceDSP const& source, ModSource mod)
{
  if (mod == ModSource::LFO1 && source.Lfos()[0].Bipolar()) return true;
  if (mod == ModSource::LFO2 && source.Lfos()[1].Bipolar()) return true;
  if (mod == ModSource::LFO3 && source.Lfos()[2].Bipolar()) return true;
  return false;
}

ModParams
ModulationParams(SourceDSP const& source, ModSource src1, ModSource src2)
{
  bool bip1 = ModBip(source, src1);
  bool bip2 = ModBip(source, src2);
  float val1 = ModVal(source, src1);
  float val2 = ModVal(source, src2);
  return ModParams(val1, bip1, val2, bip2);
}

float
Mod(float val, bool vbip, float mod, bool mbip, float amt)
{
  float range = 0.0f;
  val = EpsToZero(val);
  assert(-1.0f <= amt && amt <= 1.0f);
  assert(vbip || 0.0f <= val && val <= 1.0f);
  assert(mbip || 0.0f <= mod && mod <= 1.0f);
  assert(!vbip || -1.0f <= val && val <= 1.0f);
  assert(!mbip || -1.0f <= mod && mod <= 1.0f);
  if (amt == 0.0f) return val;
  if (!mbip && amt > 0.0f) range = 1.0f - val;
  if (!mbip && !vbip && amt < 0.0f) range = val;
  if (!mbip && vbip && amt < 0.0f) range = 1.0f + val;
  if (mbip && vbip) range = 1.0f - std::fabs(val);
  if (mbip && !vbip) range = 0.5f - std::fabs(val - 0.5f);
  float result = val + mod * amt * range;
  assert(vbip || 0.0f <= result && result <= 1.0f);
  assert(!vbip || -1.0f <= result && result <= 1.0f);
  return result;
}

} // namespace Xts