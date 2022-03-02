#ifndef XTS_DSP_MODEL_HPP
#define XTS_DSP_MODEL_HPP

#include <DSP/AudioSample.hpp>
#include <DSP/Synth/CvSample.hpp>
#include <DSP/Synth/EnvelopeSample.hpp>
#include "Model.hpp"
#include <vector>
#include <string>
#include <cstdint>
#include <complex>

namespace Xts {

typedef int PlotFlags;
inline constexpr PlotFlags PlotNone = 0x0;
inline constexpr PlotFlags PlotStereo = 0x1;
inline constexpr PlotFlags PlotBipolar = 0x2;
inline constexpr PlotFlags PlotSpectrum = 0x4;
inline constexpr PlotFlags PlotAutoRange = 0x8;
inline constexpr PlotFlags PlotNoResample = 0x10;

struct ModInput { CvSample cv1; CvSample cv2; };
struct HSplit { int pos; std::wstring marker; };
struct VSplit { float pos; std::wstring marker; };

struct PlotInput
{
  int32_t hold;
  float bpm, rate, pixels;
};

struct PlotOutput
{
  bool clip, spectrum, stereo;
  float frequency, rate, min, max;
  std::vector<float>* lSamples;
  std::vector<float>* rSamples;
  std::vector<HSplit>* hSplits;
  std::vector<VSplit>* vSplits;
  std::vector<std::complex<float>>* fftData;
  std::vector<std::complex<float>>* fftScratch;
};

} // namespace Xts
#endif // XTS_DSP_MODEL_HPP