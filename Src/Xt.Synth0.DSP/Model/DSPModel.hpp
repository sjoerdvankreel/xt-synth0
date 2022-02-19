#ifndef XTS_DSP_MODEL_HPP
#define XTS_DSP_MODEL_HPP

#include "Model.hpp"
#include <vector>
#include <string>
#include <cstdint>
#include <complex>

namespace Xts {

struct CvOutput { bool bip; float val; };
struct ModInput { CvOutput cv1; CvOutput cv2; };
struct HSplit { int pos; std::wstring marker; };
struct VSplit { float pos; std::wstring marker; };

struct CvState
{ 
  float velo;
  float envs[EnvCount];
  CvOutput lfos[LfoCount];
};

struct AudioOutput
{ 
  float l;
  float r;
  AudioOutput& operator+=(AudioOutput const& rhs)
  { l += rhs.l; r += rhs.r; return *this; }
};

struct AudioState
{
  AudioOutput units[UnitCount];
  AudioOutput filts[FilterCount];
};

struct PlotInput
{
  int32_t hold;
  XtsBool spec;
  float bpm, rate, pixels;
};

struct PlotOutput
{
  bool clip;
  bool stereo;
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