#include <DSP/Utility.hpp>

namespace Xts {

uint64_t
NextPowerOf2(uint64_t x)
{
  uint64_t result = 0;
  if(x == 0) return 0;
  if (x && !(x & (x - 1))) return x;
  while (x != 0) x >>= 1, result++;
  return 1ULL << result;
}

bool
Clip(float& val)
{
  if (val > 1.0f) return val = 1.0f, true;
  if (val < -1.0f) return val = -1.0f, true;
  return false;
}

} // namespace Xts