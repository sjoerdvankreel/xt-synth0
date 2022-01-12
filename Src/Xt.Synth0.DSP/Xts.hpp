#ifndef XTS_HPP
#define XTS_HPP

#include "DSP/PlotDSP.hpp"
#include "DSP/SequencerDSP.hpp"
#include "Model/SynthModel.hpp"
#include "Model/SequencerModel.hpp"
#include <cstdint>

#define XTS_CALL __stdcall
#define XTS_EXPORT extern "C" __declspec(dllexport) 

XTS_EXPORT void XTS_CALL XtsDSPInit(void);

XTS_EXPORT Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void);
XTS_EXPORT void XTS_CALL XtsSynthModelDestroy(Xts::SynthModel* synth);
XTS_EXPORT Xts::SequencerModel* XTS_CALL XtsSequencerModelCreate(void);
XTS_EXPORT void XTS_CALL XtsSequencerModelDestroy(Xts::SequencerModel* seq);

XTS_EXPORT Xts::PlotDSP* XTS_CALL XtsPlotDSPCreate(void);
XTS_EXPORT void XTS_CALL XtsPlotDSPDestroy(Xts::PlotDSP* dsp);
XTS_EXPORT void XTS_CALL XtsPlotDSPRender(
  Xts::PlotDSP* dsp, Xts::SynthModel const* synth, int32_t pixels, int32_t* rate, XtsBool* bipolar,
  float* frequency, float** samples, int32_t* sampleCount, int32_t** splits, int32_t* splitCount);

XTS_EXPORT Xts::SequencerDSP* XTS_CALL XtsSequencerDSPCreate(void);
XTS_EXPORT void XTS_CALL XtsSequencerDSPReset(Xts::SequencerDSP* dsp);
XTS_EXPORT void XTS_CALL XtsSequencerDSPDestroy(Xts::SequencerDSP* dsp);
XTS_EXPORT void XTS_CALL XtsSequencerDSPProcessBuffer(
  Xts::SequencerDSP* dsp, Xts::SequencerModel const* seq, Xts::SynthModel* synth,
  float rate, float* buffer, int32_t frames, int32_t* currentRow, int64_t* streamPosition);

#endif // XTS_HPP