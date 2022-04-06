#ifndef XTS_DSP_SHARED_AUDIO_SAMPLE_HPP
#define XTS_DSP_SHARED_AUDIO_SAMPLE_HPP

#include <DSP/Shared/Utility.hpp>

#include <cmath>
#include <cassert>
#include <algorithm>

namespace Xts {

template <class T>
struct AudioSample
{ 
  T left;
  T right;

  void Clear();
  T Mono() const;
  AudioSample Clip() const;
  AudioSample<T> Sanity() const;
  AudioSample<float> ToFloat() const;
  AudioSample<double> ToDouble() const;
  AudioSample& operator+=(AudioSample s);
};

typedef AudioSample<float> FloatSample;
typedef AudioSample<double> DoubleSample;

template <class T>
inline T
AudioSample<T>::Mono() const
{ return left + right; }

template <class T>
inline AudioSample<T>
AudioSample<T>::Sanity() const
{
  Xts::Sanity(left);
  Xts::Sanity(right);
  return *this;
}

template <class T>
inline void
AudioSample<T>::Clear()
{ left = right = static_cast<T>(0.0); }

template <class T>
inline AudioSample<T>
operator*(AudioSample<T> x, T y)
{ return { x.left * y, x.right * y }; }

template <class T>
inline AudioSample<T>
operator*(T x, AudioSample<T> y)
{ return { x * y.left, x * y.right }; }

template <class T>
inline AudioSample<T>
operator+(AudioSample<T> x, AudioSample<T> y)
{ return { x.left + y.left, x.right + y.right }; }

template <class T>
inline AudioSample<T>
operator-(AudioSample<T> x, AudioSample<T> y)
{ return { x.left - y.left, x.right - y.right }; }

template <class T>
inline AudioSample<T>
operator*(AudioSample<T> x, AudioSample<T> y)
{ return { x.left * y.left, x.right * y.right }; }

template <class T>
inline AudioSample<T>&
AudioSample<T>::operator+=(AudioSample<T> s)
{ left += s.left; right += s.right; return *this; }

template <class T>
inline AudioSample<T>
AudioSample<T>::Clip() const
{ return { std::clamp(left, -1.0, 1.0), std::clamp(right, -1.0, 1.0) }; }

template <>
inline AudioSample<float>
AudioSample<double>::ToFloat() const
{ return AudioSample<float> { static_cast<float>(left), static_cast<float>(right) }; }

template <>
inline AudioSample<double>
AudioSample<float>::ToDouble() const
{ return AudioSample<double> { static_cast<double>(left), static_cast<double>(right) }; }

} // namespace Xts
#endif // XTS_DSP_SHARED_AUDIO_SAMPLE_HPP