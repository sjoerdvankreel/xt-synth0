#include "DSP.hpp"
#include <cassert>
#include <numeric>

namespace Xts {

static void
FftCombine(std::vector<std::complex<float>>& xs, size_t stride)
{
  for (size_t i = 0; i < xs.size() / stride; i += stride)
  {
    auto polar = std::polar(1.0f, -2.0f * PI * i / (xs.size() / stride));
    std::complex<float> t = polar * xs[i + 1];
    xs[i] = xs[i] + t;
    xs[i + xs.size() / stride] = xs[i] - t;
  }
}

static void
Fft(std::vector<std::complex<float>>& x, size_t start, size_t stride)
{
  if(stride >= x.size()) return;
  Fft(x, 0, stride * 2);
  Fft(x, 1, stride * 2);
  FftCombine(x, stride * 2);
}

void 
InplacePaddedFft(std::vector<float>& x, std::vector<std::complex<float>>& scratch)
{
  for(size_t i = x.size(); i < NextPow2(x.size()); i++)
    x.emplace_back(0.0f);
  scratch.clear();
  for(size_t i = 0; i < x.size(); i++)
    scratch.emplace_back(std::complex<float>(x[i], 0.0f));
  Fft(scratch, 0, 2);
  Fft(scratch, 1, 2);
  FftCombine(scratch, 2);
  for(size_t i = 0; i < x.size(); i++)
    x[i] = scratch[i].real();
}

} // namespace Xts