#ifndef XTS_HPP
#define XTS_HPP

#include <DSP/Shared/Plot.hpp>
#include <Model/Shared/Model.hpp>

#include <vector>
#include <memory>
#include <cstdint>
#include <complex>

#define XTS_CALL __stdcall
#define XTS_EXPORT extern "C" __declspec(dllexport)

namespace Xts {
class PlotDSP;
class SequencerDSP;

struct ParamInfo;
struct SynthModel;
struct ParamBinding;
struct SyncStepModel;
struct SequencerModel;
} // namespace Xts

struct XTS_ALIGN SequencerState
{
  int32_t row;
  int32_t rate;
  int32_t frames;
  int32_t voices;
  XtsBool end;
  XtsBool clip;
  XtsBool exhausted;
  int32_t pad__;
  float* buffer;
  int64_t position;
  Xts::SynthModel* synth;
  Xts::SequencerModel const* sequencer;
};

struct XTS_ALIGN PlotState
{
  XtsBool spec;
  XtsBool clip;
  XtsBool stereo;
  float* lSamples;
  float* rSamples;
  float* vSplitVals;
  int32_t* hSplitVals;
  int32_t bpm, pixels;
  float freq, rate, min, max;
  wchar_t const** vSplitMarkers;
  wchar_t const** hSplitMarkers;
  int32_t sampleCount, hSplitCount, vSplitCount;
  Xts::SynthModel const* synth;
  std::vector<float>* lSampleData;
  std::vector<float>* rSampleData;
  std::vector<float>* vSplitValData;
  std::vector<int32_t>* hSplitValData;
  std::vector<Xts::VerticalMarker>* vSplitData;
  std::vector<Xts::HorizontalMarker>* hSplitData;
  std::vector<wchar_t const*>* vSplitMarkerData;
  std::vector<wchar_t const*>* hSplitMarkerData;
  std::vector<std::complex<float>>* fftData;
  std::vector<std::complex<float>>* fftScratch;
public:
  PlotState() = default;
  PlotState(PlotState const&) = delete;
};

XTS_EXPORT SequencerState* XTS_CALL XtsSequencerStateCreate(void);
XTS_EXPORT Xts::SequencerDSP* XTS_CALL XtsSequencerDSPCreate(void);
XTS_EXPORT PlotState* XTS_CALL XtsPlotStateCreate(void);
XTS_EXPORT Xts::SequencerModel* XTS_CALL XtsSequencerModelCreate(void);
XTS_EXPORT Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void);
XTS_EXPORT Xts::ParamBinding* XTS_CALL XtsParamBindingCreate(int32_t count);

XTS_EXPORT void XTS_CALL XtsSequencerDSPDestroy(Xts::SequencerDSP* dsp);
XTS_EXPORT void XTS_CALL XtsSequencerStateDestroy(SequencerState* state);
XTS_EXPORT void XTS_CALL XtsPlotStateDestroy(PlotState* state);
XTS_EXPORT void XTS_CALL XtsSequencerModelDestroy(Xts::SequencerModel* model);
XTS_EXPORT void XTS_CALL XtsSynthModelDestroy(Xts::SynthModel* model);
XTS_EXPORT void XTS_CALL XtsParamBindingDestroy(Xts::ParamBinding* binding);

XTS_EXPORT void XTS_CALL XtsPlotDSPRender(PlotState* state);
XTS_EXPORT void XTS_CALL XtsSequencerDSPRender(Xts::SequencerDSP* dsp, SequencerState* state);
XTS_EXPORT void XTS_CALL XtsSynthModelInit(Xts::ParamInfo* infos, int32_t infoCount, Xts::SyncStepModel* steps, int32_t stepCount);
XTS_EXPORT void XTS_CALL XtsSequencerDSPInit(Xts::SequencerDSP* dsp, Xts::SequencerModel const* model, Xts::SynthModel const* synth, Xts::ParamBinding const* binding);

#endif // XTS_HPP