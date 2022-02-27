#ifndef XTS_DSP_MODEL_HPP
#define XTS_DSP_MODEL_HPP

#include "Model.hpp"
#include <vector>
#include <string>
#include <cstdint>
#include <complex>

namespace Xts {

typedef int PlotFlags;
inline constexpr PlotFlags PlotNone = 0x0;
inline constexpr PlotFlags PlotSpec = 0x1;
inline constexpr PlotFlags PlotStereo = 0x2;
inline constexpr PlotFlags PlotBipolar = 0x4;
inline constexpr PlotFlags PlotAutoRange = 0x8;
inline constexpr PlotFlags PlotNoResample = 0x10;

struct CvOutput { bool bip; float val; };
struct ModInput { CvOutput cv1; CvOutput cv2; };
struct HSplit { int pos; std::wstring marker; };
struct VSplit { float pos; std::wstring marker; };
enum class EnvStage { Dly, A, Hld, D, S, R, End };
struct EnvOutput { float val; EnvStage stage; bool staged; };

struct CvState
{ 
  float velo;
  CvOutput lfos[LfoCount];
  EnvOutput envs[EnvCount];
};

template <class T>
struct AudioOutput
{ 
  T l, r;
  T Mono() const { return l + r; }
  void Clear() { l = r = static_cast<T>(0.0); }
  AudioOutput operator*(T s) const { return { l * s, r * s }; }
  AudioOutput operator+(AudioOutput s) const { return { l + s.l, r + s.r }; }
  AudioOutput operator-(AudioOutput s) const { return { l - s.l, r - s.r }; }
  AudioOutput operator*(AudioOutput s) const { return { l * s.l, r * s.r }; }
  AudioOutput& operator+=(AudioOutput const& rhs) { l += rhs.l; r += rhs.r; return *this; }
  AudioOutput<float> ToFloat() const { return AudioOutput<float> { static_cast<float>(l), static_cast<float>(r) }; }
  AudioOutput<double> ToDouble() const { return AudioOutput<double> { static_cast<double>(l), static_cast<double>(r) }; }
};

typedef AudioOutput<float> FAudioOutput;
typedef AudioOutput<double> DAudioOutput;

struct AudioState
{
  FAudioOutput units[UnitCount];
  FAudioOutput filts[FilterCount];
};

struct PlotInput
{
  int32_t hold;
  float bpm, rate, pixels;
};

struct PlotOutput
{
  bool clip, spec, stereo;
  float freq, rate, min, max;
  std::vector<float>* lSamples;
  std::vector<float>* rSamples;
  std::vector<HSplit>* hSplits;
  std::vector<VSplit>* vSplits;
  std::vector<std::complex<float>>* fftData;
  std::vector<std::complex<float>>* fftScratch;
};

} // namespace Xts
#endif // XTS_DSP_MODEL_HPP