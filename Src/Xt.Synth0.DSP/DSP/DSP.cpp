#include "DSP.hpp"
#include <cassert>

namespace Xts {

static void
Fft(std::vector<std::complex<float>>& x)
{
}

void 
Fft(std::vector<float>& x, std::vector<std::complex<float>>& scratch)
{
  assert(x.size() == NextPow2(x.size()));
  scratch.clear();
  for(size_t i = 0; i < x.size(); i++)
    scratch.emplace_back(std::complex<float>(x[i], 0.0f));
  Fft(scratch);
  for(size_t i = 0; i < x.size(); i++)
    x[i] = scratch[i].real();
}

} // namespace Xts