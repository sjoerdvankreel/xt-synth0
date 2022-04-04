#ifndef XTS_DSP_SHARED_PRNG_HPP
#define XTS_DSP_SHARED_PRNG_HPP

#include <limits>
#include <cstdint>

// https://en.wikipedia.org/wiki/Lehmer_random_number_generator

namespace Xts {

class Prng {
  uint32_t _state;
public:
  float Next();
public:
  Prng() : _state(1U) {}
  Prng(uint32_t seed): _state(seed) {}
};

inline float 
Prng::Next()
{ 
  _state = static_cast<uint64_t>(_state) * 48271 % 0x7fffffff; 
  return static_cast<float>(_state) / static_cast<float>(std::numeric_limits<int32_t>::max());
}

} // namespace Xts
#endif // XTS_DSP_SHARED_PRNG_HPP