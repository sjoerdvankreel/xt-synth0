#ifndef XTS_API_XTS_HPP
#define XTS_API_XTS_HPP

#include <DSP/Shared/Plot.hpp>
#include <Model/Shared/Model.hpp>

#include <vector>
#include <memory>
#include <cstdint>
#include <complex>

#define XTS_CALL __stdcall
#define XTS_EXPORT extern "C" __declspec(dllexport)

namespace Xts {
struct ParamInfo;
struct SynthModel;
struct ParamBinding;
struct SyncStepModel;

class SequencerDSP;
struct SequencerModel;
struct SequencerOutput;
} // namespace Xts

struct XTS_ALIGN PlotState
{
  float min;
  float max;
  float rate;
  float frequency;

  XtsBool clip;
  XtsBool stereo;
  XtsBool spectrum;

  int32_t bpm;
  int32_t pixels;
  int32_t sampleCount;
  int32_t verticalCount;
  int32_t horizontalCount;

  float* left;
  float* right;
  float* verticalValues;
  wchar_t const** verticalTexts;
  int32_t* horizontalValues;
  wchar_t const** horizontalTexts;

  std::vector<float>* leftData;
  std::vector<float>* rightData;

  Xts::SynthModel const* synth;
  std::vector<std::complex<float>>* fft;
  std::vector<std::complex<float>>* scratch;

  std::vector<float>* verticalValueData;
  std::vector<wchar_t const*>* verticalTextData;
  std::vector<Xts::VerticalMarker>* verticalData;

  std::vector<int32_t>* horizontalValueData;
  std::vector<wchar_t const*>* horizontalTextData;
  std::vector<Xts::HorizontalMarker>* horizontalData;
};

XTS_EXPORT void XTS_CALL
XtsPlotDSPRender(PlotState* state);
XTS_EXPORT PlotState* XTS_CALL
XtsPlotStateCreate(void);
XTS_EXPORT void XTS_CALL
XtsPlotStateDestroy(PlotState* state);

XTS_EXPORT Xts::SynthModel* XTS_CALL
XtsSynthModelCreate(void);
XTS_EXPORT Xts::ParamBinding* XTS_CALL
XtsParamBindingCreate(int32_t count);
XTS_EXPORT Xts::SequencerModel* XTS_CALL
XtsSequencerModelCreate(void);

XTS_EXPORT void XTS_CALL
XtsSynthModelDestroy(Xts::SynthModel* model);
XTS_EXPORT void XTS_CALL
XtsParamBindingDestroy(Xts::ParamBinding* binding);
XTS_EXPORT void XTS_CALL
XtsSequencerModelDestroy(Xts::SequencerModel* model);

XTS_EXPORT void XTS_CALL
XtsSynthModelInit(Xts::ParamInfo* params, int32_t count);
XTS_EXPORT void XTS_CALL
XtsSyncStepModelInit(Xts::SyncStepModel* steps, int32_t count);

XTS_EXPORT void XTS_CALL
XtsSequencerDSPDestroy(Xts::SequencerDSP* dsp);
XTS_EXPORT Xts::SequencerOutput const* XTS_CALL
XtsSequencerDSPRender(Xts::SequencerDSP* dsp, int32_t frames, float rate);
XTS_EXPORT Xts::SequencerDSP* XTS_CALL
XtsSequencerDSPCreate(Xts::SequencerModel const* model, Xts::SynthModel const* synth, Xts::ParamBinding const* binding, size_t frames);

#endif // XTS_API_XTS_HPP