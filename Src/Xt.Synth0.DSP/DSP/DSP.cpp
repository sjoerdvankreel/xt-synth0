#include "DSP.hpp"

namespace Xts {

static void
Fft(
  std::complex<float>* x, 
  std::complex<float>* scratch, 
  size_t count)
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
Fft(
  std::vector<float> const& x, 
  std::vector<std::complex<float>>& fft, 
  std::vector<std::complex<float>>& scratch)
{
  assert(x.size() == NextPow2(x.size()));
  fft.resize(x.size());
  scratch.resize(x.size());
  for(size_t i = 0; i < x.size(); i++)
    fft[i] = std::complex<float>(x[i], 0.0f);
  Fft(fft.data(), scratch.data(), x.size());
}

} // namespace Xts