#ifndef XTS_DSP_SHARED_PRNG_HPP
#define XTS_DSP_SHARED_PRNG_HPP

#include <cstdint>

// https://en.wikipedia.org/wiki/Lehmer_random_number_generator

namespace Xts {

class Prng {
  uint32_t _state;
public:
  Prng() : _state(1U) {}
  Prng(uint32_t seed): _state(seed) {}
  uint32_t Next() { return _state = static_cast<uint64_t>(_state) * 48271 % 0x7fffffff; }
};

} // namespace Xts
#endif // XTS_DSP_SHARED_PRNG_HPP