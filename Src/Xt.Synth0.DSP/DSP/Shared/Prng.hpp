#ifndef XTS_DSP_SHARED_PRNG_HPP
#define XTS_DSP_SHARED_PRNG_HPP

#include <cstdint>

// https://stackoverflow.com/questions/1640258/need-a-fast-random-generator-for-c

namespace Xts {

class Prng {
  uint32_t _state;
public:
  Prng() : _state(0) {}
  Prng(uint32_t seed): _state(seed) {}
  int32_t Next() { _state = (214013 * _state + 2531011); return (_state >> 16) & 0x7FFF; }
};

} // namespace Xts
#endif // XTS_DSP_SHARED_PRNG_HPP