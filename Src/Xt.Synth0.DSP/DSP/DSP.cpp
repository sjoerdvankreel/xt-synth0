#include "DSP.hpp"
#include <cassert>
#include <numeric>

namespace Xts {

// TODO fft not dft
void
Fft(std::vector<float>& x, std::vector<std::complex<float>>& scratch)
{
  int N = x.size();
  std::vector<std::complex<float>> X((size_t)N, std::complex<float>());
  for (int k = 0; k < N; k++)
  {
    X[k] = std::complex<float>(0.0f, 0.0f);
    for (int n = 0; n < N; n++)
    {
      std::complex<float> temp = std::polar<float>(1, -2 * PI * n * k / N);
      temp *= x[n];
      X[k] += temp;
    }
  }
  for(int i = 0; i < N; i++) x[i]=X[i].real();
}

} // namespace Xts