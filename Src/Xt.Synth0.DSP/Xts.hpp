#ifndef XTS_HPP
#define XTS_HPP

#include "DSP/SeqDSP.hpp"
#include "DSP/PlotDSP.hpp"
#include "Model/SeqModel.hpp"
#include "Model/SynthModel.hpp"
#include <cstdint>

#define XTS_CALL __stdcall
#define XTS_EXPORT extern "C" __declspec(dllexport) 

XTS_EXPORT void XTS_CALL XtsDSPInit(void);

XTS_EXPORT Xts::SeqModel* XTS_CALL XtsSeqModelCreate(void);
XTS_EXPORT Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void);
XTS_EXPORT void XTS_CALL XtsSeqModelDestroy(Xts::SeqModel* seq);
XTS_EXPORT void XTS_CALL XtsSynthModelDestroy(Xts::SynthModel* synth);

XTS_EXPORT Xts::SeqDSP* XTS_CALL XtsSeqDSPCreate(void);
XTS_EXPORT void XTS_CALL XtsSeqDSPInit(Xts::SeqDSP* dsp);
XTS_EXPORT void XTS_CALL XtsSeqDSPDestroy(Xts::SeqDSP* dsp);
XTS_EXPORT void XTS_CALL XtsSeqDSPProcessBuffer(Xts::SeqDSP* dsp, Xts::SeqState* state);

XTS_EXPORT Xts::PlotDSP* XTS_CALL XtsPlotDSPCreate(void);
XTS_EXPORT void XTS_CALL XtsPlotDSPDestroy(Xts::PlotDSP* dsp);
XTS_EXPORT void XTS_CALL XtsPlotDSPRender(
  Xts::PlotDSP* dsp, Xts::SynthModel const* synth, int32_t pixels, int32_t* rate, XtsBool* bipolar,
  float* frequency, float** samples, int32_t* sampleCount, int32_t** splits, int32_t* splitCount);

#endif // XTS_HPP