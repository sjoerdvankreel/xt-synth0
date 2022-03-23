#ifndef XTS_DSP_SHARED_PLOT_HPP
#define XTS_DSP_SHARED_PLOT_HPP

#include <Model/Shared/Model.hpp>
#include <DSP/Shared/EnvSample.hpp>

#include <vector>
#include <memory>
#include <complex>

namespace Xts {

struct StagedParams
{
  bool stereo;
  bool bipolar;
  bool allowResample;
  bool allowSpectrum;
};

struct PeriodicParams
{
  int periods;
  bool bipolar;
  bool autoRange;
  bool allowResample;
};

struct VerticalMarker
{ 
  float position; 
  std::wstring text; 
};

struct HorizontalMarker
{
  int position;
  std::wstring text;
};

struct XTS_ALIGN PlotInput
{
  float bpm;
  float rate;
  float pixels;
  XtsBool spectrum;
};

struct XTS_ALIGN PlotOutput
{
  float min;
  float max;
  float rate;
  float frequency;
  XtsBool clip;
  XtsBool stereo;
  XtsBool spectrum;
  int32_t pad__;
};

struct XTS_ALIGN PlotResult
{
  float* left;
  float* right;
  int32_t sampleCount;
  int32_t verticalCount;
  int32_t horizontalCount;
  int32_t pad__;
  float* verticalPositions;
  int32_t* horizontalPositions;
  wchar_t const** verticalTexts;
  wchar_t const** horizontalTexts;
};

struct XTS_ALIGN PlotData
{
  std::vector<float> left;
  std::vector<float> right;
  std::vector<VerticalMarker> vertical;
  std::vector<HorizontalMarker> horizontal;
};

struct XTS_ALIGN PlotScratch
{
  std::vector<float> verticalPositions;
  std::vector<int32_t> horizontalPositions;
  std::vector<wchar_t const*> verticalTexts;
  std::vector<wchar_t const*> horizontalTexts;
  std::vector<std::complex<float>> fft;
  std::vector<std::complex<float>> fftScratch;
};

struct XTS_ALIGN PlotState
{
  int32_t hold;
  int32_t pad__;
  PlotOutput output;
  PlotResult result;
  PlotData* data;
  PlotScratch* scratch;
};

class Plot
{
public:
  virtual ~Plot() {}
protected:
  void DoRender(PlotInput const& input, PlotState& state);
  virtual void RenderCore(PlotInput const& input, int hold, PlotOutput& output, PlotData& data) = 0;
};

class PeriodicPlot: public Plot
{
public:
  virtual ~PeriodicPlot() {}
  virtual float Next() = 0;
  virtual PeriodicParams Params() const = 0;
  virtual void Init(float bpm, float rate) = 0;
  virtual float Frequency(float bpm, float rate) const = 0;
protected:
  void RenderCore(PlotInput const& input, int hold, PlotOutput& output, PlotData& data);
};

class StagedPlot: public Plot
{
public:
  virtual ~StagedPlot() {}
  virtual void Next() = 0;
  virtual EnvSample Release() = 0;
  virtual void Init(float bpm, float rate) = 0;
public:
  virtual void Start() = 0;
  virtual bool End() const = 0;
  virtual float Left() const = 0;
  virtual float Right() const = 0;
  virtual EnvSample EnvOutput() const = 0;
  virtual StagedParams Params() const = 0;
  virtual float ReleaseSamples(float bpm, float rate) const = 0;
protected:
  void RenderCore(PlotInput const& input, int hold, PlotOutput& output, PlotData& data);
};

} // namespace Xts
#endif // XTS_DSP_SHARED_PLOT_HPP