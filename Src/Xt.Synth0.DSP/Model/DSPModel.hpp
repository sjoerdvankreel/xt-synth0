#ifndef XTS_DSP_MODEL_HPP
#define XTS_DSP_MODEL_HPP

#include "Model.hpp"
#include <vector>
#include <string>
#include <cstdint>
#include <complex>

namespace Xts {

inline constexpr wchar_t UnicodePi = L'\u03C0';
inline constexpr wchar_t UnicodeOneHalf = L'\u00BD';
inline constexpr wchar_t UnicodeOneEight = L'\u215B';
inline constexpr wchar_t UnicodeOneQuarter = L'\u00BC';
inline constexpr wchar_t UnicodeThreeQuarter = L'\u00BE';

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

struct AudioOutput
{ 
  float l;
  float r;
  void Clear() { l = r = 0.0f; }
  float Mono() const { return l + r; }
  AudioOutput operator*(float s) const { return { l * s, r * s }; }
  AudioOutput operator+(AudioOutput s) const { return { l + s.l, r + s.r }; }
  AudioOutput operator-(AudioOutput s) const { return { l - s.l, r - s.r }; }
  AudioOutput operator*(AudioOutput s) const { return { l * s.l, r * s.r }; }
  AudioOutput& operator+=(AudioOutput const& rhs) { l += rhs.l; r += rhs.r; return *this; }
};

struct AudioState
{
  AudioOutput units[UnitCount];
  AudioOutput filts[FilterCount];
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