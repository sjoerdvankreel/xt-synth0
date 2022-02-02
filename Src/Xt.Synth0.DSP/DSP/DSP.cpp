#include "DSP.hpp"
#include <cassert>
#include <numeric>

namespace Xts {

// TODO fft not dft
void
Fft(std::vector<float>& yn, std::vector<std::complex<float>>& scratch)
{
  int N = yn.size();
  std::vector<float> xn = yn;
  std::vector<float> Xr;
  std::vector<float> Xi;
  Xr.resize(xn.size());
  Xi.resize(xn.size());

  for(int i = 0; i < N; i++)xn[i]=xn[i]*2-1;

  int len = N;
  for (int k = 0; k < N; k++) {
    Xr[k] = 0;
    Xi[k] = 0;
    for (int n = 0; n < len; n++) {
      Xr[k]
        = (Xr[k]
          + xn[n] * cos(2 * 3.141592 * k * n / N));
      Xi[k]
        = (Xi[k]
          + xn[n] * sin(2 * 3.141592 * k * n / N));
    }
  }
  float max = 0;
  for(int i = 0; i < N; i++) 
  {
    xn[i]=sqrt(Xr[i]* Xr[i] + Xi[i]*Xi[i]);
    max = xn[i]>max?xn[i]:max;
  }
  for (int i = 0; i < N; i++)xn[i]/=max;
  yn.clear();
  for(int i = 0; i < N/2;i++)yn.push_back(xn[i]);
  
  int i=0;
  i++;
}

} // namespace Xts