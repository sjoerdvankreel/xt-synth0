#ifndef XTS_HPP
#define XTS_HPP

#include "Model/Model.hpp"
#include "Model/DSPModel.hpp"

#include <vector>
#include <memory>
#include <cstdint>
#include <complex>

#define XTS_CALL __stdcall
#define XTS_EXPORT extern "C" __declspec(dllexport)

namespace Xts {
class SeqDSP;
class PlotDSP;
struct SyncStep;
struct SeqModel;
struct ParamInfo;
struct SynthModel;
struct VoiceBinding;
} // namespace Xts

struct XTS_ALIGN SeqState
{
  int32_t row, voices;
  int32_t clip, exhausted;
  int32_t rate, frames;
  int32_t end, pad__;
  int64_t pos;
  float* buffer;
  Xts::SynthModel* synth;
  Xts::SeqModel const* seq;
public:
  SeqState() = default;
  SeqState(SeqState const&) = delete;
};

struct XTS_ALIGN PlotState
{
  XtsBool clip;
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
  std::vector<Xts::VSplit>* vSplitData;
  std::vector<Xts::HSplit>* hSplitData;
  std::vector<wchar_t const*>* vSplitMarkerData;
  std::vector<wchar_t const*>* hSplitMarkerData;
  std::vector<std::complex<float>>* fftData;
  std::vector<std::complex<float>>* fftScratch;
public:
  PlotState() = default;
  PlotState(PlotState const&) = delete;
};

XTS_EXPORT SeqState* XTS_CALL XtsSeqStateCreate(void);
XTS_EXPORT Xts::SeqDSP* XTS_CALL XtsSeqDSPCreate(void);
XTS_EXPORT PlotState* XTS_CALL XtsPlotStateCreate(void);
XTS_EXPORT Xts::SeqModel* XTS_CALL XtsSeqModelCreate(void);
XTS_EXPORT Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void);
XTS_EXPORT Xts::VoiceBinding* XTS_CALL XtsVoiceBindingCreate(void);

XTS_EXPORT void XTS_CALL XtsSeqDSPDestroy(Xts::SeqDSP* dsp);
XTS_EXPORT void XTS_CALL XtsSeqStateDestroy(SeqState* state);
XTS_EXPORT void XTS_CALL XtsPlotStateDestroy(PlotState* state);
XTS_EXPORT void XTS_CALL XtsSeqModelDestroy(Xts::SeqModel* model);
XTS_EXPORT void XTS_CALL XtsSynthModelDestroy(Xts::SynthModel* model);
XTS_EXPORT void XTS_CALL XtsVoiceBindingDestroy(Xts::VoiceBinding* binding);

XTS_EXPORT void XTS_CALL XtsPlotDSPRender(PlotState* state);
XTS_EXPORT void XTS_CALL XtsSeqDSPRender(Xts::SeqDSP* dsp, SeqState* state);
XTS_EXPORT void XTS_CALL XtsSynthModelInit(Xts::ParamInfo* infos, int32_t infoCount, Xts::SyncStep* steps, int32_t stepCount);
XTS_EXPORT void XTS_CALL XtsSeqDSPInit(Xts::SeqDSP* dsp, Xts::SeqModel const* model, Xts::SynthModel const* synth, Xts::VoiceBinding const* binding);

#endif // XTS_HPP