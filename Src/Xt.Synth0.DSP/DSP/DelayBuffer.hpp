#ifndef XTS_DSP_DELAY_BUFFER_HPP
#define XTS_DSP_DELAY_BUFFER_HPP

#include <cstring>
#include <cassert>

namespace Xts {

template <class T, size_t N>
class DelayBuffer
{
  T _buffer[N];
  size_t _head;
public:
  void Clear();
  void Push(T val);
  T Get(size_t pos) const;
};

template <class T, size_t N>
inline void
DelayBuffer<T, N>::Push(T val)
{
  _buffer[_head++] = val;
  _head %= N;
}

template <class T, size_t N>
inline void
DelayBuffer<T, N>::Clear()
{ std::memset(_buffer, 0, sizeof(_buffer)); }

template <class T, size_t N>
inline T 
DelayBuffer<T, N>::Get(size_t pos) const
{ 
  assert(0 <= pos && pos < N);
  return _buffer[(_head + N - pos - 1) % N]; 
}

} // namespace Xts
#endif // XTS_DSP_DELAY_BUFFER_HPP