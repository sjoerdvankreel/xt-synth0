#ifndef XTS_DSP_SHARED_PLOT_HPP
#define XTS_DSP_SHARED_PLOT_HPP

#include <DSP/Shared/EnvSample.hpp>

#include <vector>
#include <memory>
#include <complex>

namespace Xts {

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

struct StagedParams
{
  bool stereo;
  bool bipolar;
  bool allowResample;
};

struct PeriodicParams
{
  int periods;
  bool bipolar;
  bool autoRange;
  bool allowResample;
};

class PeriodicPlot
{
public:
  virtual float Next() = 0;
  virtual PeriodicParams Params() const = 0;
  virtual void Init(float bpm, float rate) = 0;
  virtual float Frequency(float bpm, float rate) const = 0;

  virtual ~PeriodicPlot() {}
  void RenderCore(PlotInput const& input, PlotOutput& output);
};

class StagedPlot
{
public:
  virtual ~StagedPlot() {}
  virtual void Next() = 0;
  virtual EnvSample Release() = 0;
  virtual void Init(float bpm, float rate) = 0;

  virtual bool End() const = 0;
  virtual float Left() const = 0;
  virtual float Right() const = 0;
  virtual EnvSample EnvOutput() const = 0;
  virtual StagedParams Params() const = 0;
  virtual float ReleaseSamples(float bpm, float rate) const = 0;

  void RenderCore(PlotInput const& input, int hold, PlotOutput& output);
};

} // namespace Xts
#endif // XTS_DSP_SHARED_PLOT_HPP