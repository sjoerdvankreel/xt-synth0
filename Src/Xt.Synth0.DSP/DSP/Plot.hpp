#ifndef XTS_DSP_PLOT_HPP
#define XTS_DSP_PLOT_HPP

#include <vector>
#include <memory>
#include <complex>

namespace Xts {

typedef int PlotFlags;
inline constexpr PlotFlags PlotNone = 0x0;
inline constexpr PlotFlags PlotStereo = 0x1;
inline constexpr PlotFlags PlotBipolar = 0x2;
inline constexpr PlotFlags PlotSpectrum = 0x4;
inline constexpr PlotFlags PlotAutoRange = 0x8;
inline constexpr PlotFlags PlotNoResample = 0x10;

struct HSplit { int pos; std::wstring marker; };
struct VSplit { float pos; std::wstring marker; };

struct PlotInput
{
  float bpm;
  float rate;
  float pixels;
  bool spectrum;
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

class CycledPlot
{
public:
  virtual ~CycledPlot() {}
  virtual float Next() = 0;
  virtual void Init(float bpm, float rate) = 0;

  virtual int Cycles() const = 0;
  virtual bool Bipolar() const = 0;
  virtual bool AutoRange() const = 0;
  virtual float Frequency(float bpm, float rate) const = 0;

  void Render(PlotInput const& input, PlotOutput& output);
};

} // namespace Xts
#endif // XTS_DSP_PLOT_HPP