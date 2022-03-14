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

XTS_EXPORT class Xts::SequencerDSP* XTS_CALL 
XtsSequencerDSPCreate(struct Xts::SequencerModel const* model, struct Xts::SynthModel const* synth, struct Xts::ParamBinding const* binding, size_t frames);
XTS_EXPORT PlotState* XTS_CALL XtsPlotStateCreate(void);
XTS_EXPORT struct Xts::SequencerModel* XTS_CALL XtsSequencerModelCreate(void);
XTS_EXPORT struct Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void);
XTS_EXPORT struct Xts::ParamBinding* XTS_CALL XtsParamBindingCreate(int32_t count);

XTS_EXPORT void XTS_CALL XtsSequencerDSPDestroy(class Xts::SequencerDSP* dsp);
XTS_EXPORT void XTS_CALL XtsPlotStateDestroy(PlotState* state);
XTS_EXPORT void XTS_CALL XtsSequencerModelDestroy(struct Xts::SequencerModel* model);
XTS_EXPORT void XTS_CALL XtsSynthModelDestroy(struct Xts::SynthModel* model);
XTS_EXPORT void XTS_CALL XtsParamBindingDestroy(struct Xts::ParamBinding* binding);

XTS_EXPORT void XTS_CALL XtsPlotDSPRender(PlotState* state);
XTS_EXPORT void XTS_CALL XtsSynthModelInit(struct Xts::ParamInfo* params, int32_t count);
XTS_EXPORT void XTS_CALL XtsSyncStepModelInit(struct Xts::SyncStepModel* steps, int32_t count);

struct Xts::SequencerOutput const* XTS_CALL
XtsSequencerDSPRender(Xts::SequencerDSP* dsp, int32_t frames, float rate);

#endif // XTS_HPP