#ifndef XTS_DSP_MODEL_HPP
#define XTS_DSP_MODEL_HPP

#include "Model.hpp"
#include <vector>
#include <string>
#include <cstdint>
#include <complex>

namespace Xts {

struct CVOutput { bool bip; float val; };
struct AudioOutput { float l; float r; };
struct ModInput { CVOutput cv1; CVOutput cv2; };

struct HSplit { int pos; std::wstring marker; };
struct VSplit { float pos; std::wstring marker; };

struct PlotInput
{
  int32_t hold;
  XtsBool spec;
  float bpm, rate, pixels;
};

struct PlotOutput
{
  bool clip;
  int channel;
  float freq, rate, min, max;
  std::vector<float>* samples;
  std::vector<HSplit>* hSplits;
  std::vector<VSplit>* vSplits;
  std::vector<std::complex<float>>* fftData;
  std::vector<std::complex<float>>* fftScratch;
};

} // namespace Xts
#endif // XTS_DSP_MODEL_HPP