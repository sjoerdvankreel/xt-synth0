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

struct XtsSequencer;

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

XTS_EXPORT void XTS_CALL
XtsSynthModelInit(Xts::ParamInfo* params, int32_t count);
XTS_EXPORT void XTS_CALL
XtsSyncStepModelInit(Xts::SyncStepModel* steps, int32_t count);

XTS_EXPORT void XTS_CALL
XtsSequencerDestroy(XtsSequencer* sequencer);
XTS_EXPORT Xts::SequencerOutput const* XTS_CALL
XtsSequencerRender(XtsSequencer* sequencer, int32_t frames);
XTS_EXPORT XtsSequencer* XTS_CALL
XtsSequencerCreate(int32_t params, int32_t frames, float rate);

#endif // XTS_API_XTS_HPP