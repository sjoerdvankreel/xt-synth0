#include "DSP.hpp"

namespace Xts {

static void
Fft(
  std::complex<double>* x,
  std::complex<double>* scratch,
  size_t count)
{
  assert(count == NextPow2(count));
  if(count < 2) return;  
  std::complex<double>* even = scratch;
  std::complex<double>* odd = scratch + count / 2;
  for(size_t i = 0; i < count / 2; i++) even[i] = x[i * 2];
  for(size_t i = 0; i < count / 2; i++) odd[i] = x[i * 2 + 1];
  Fft(odd, x, count / 2);
  Fft(even, x, count / 2);
  for (size_t i = 0; i < count / 2; i++)
  {
    double im = -2.0f * PID * i / count;
    std::complex<double> t = std::polar(1.0, im) * odd[i];
    x[i] = even[i] + t;
    x[i + count/2] = even[i] - t;
  }
}

void 
Fft(
  std::vector<float> const& x, 
  std::vector<std::complex<double>>& fft,
  std::vector<std::complex<double>>& scratch)
{
  assert(x.size() == NextPow2(x.size()));
  fft.resize(x.size());
  scratch.resize(x.size());
  for(size_t i = 0; i < x.size(); i++)
    fft[i] = std::complex<double>(x[i], 0.0);
  Fft(fft.data(), scratch.data(), x.size());
}

} // namespace Xts