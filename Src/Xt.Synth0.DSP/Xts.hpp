#ifndef XTS_HPP
#define XTS_HPP

#include "DSP/SeqDSP.hpp"
#include "DSP/PlotDSP.hpp"
#include "Model/SeqModel.hpp"
#include "Model/SynthModel.hpp"
#include <cstdint>


struct XTS_ALIGN SeqState
{
  float rate;
  int32_t voices;
  XtsBool clip;
  int32_t frames;
  int32_t currentRow;
  XtsBool exhausted;
  int64_t streamPosition;
  float* buffer;
  SynthModel* synth;
  SeqModel const* seq;
  SeqState() = default;
};

struct XTS_ALIGN PlotInput
{
  int32_t rate;
  int32_t pixels;
  SynthModel const* synth;
  PlotInput() = default;
};

struct XTS_ALIGN PlotOutput0
{
  float freq;
  int32_t rate;
  XtsBool clip;
  XtsBool bipolar;
  int32_t splitCount;
  int32_t sampleCount;
  float* samples;
  int32_t* splits;
  PlotOutput0() = default;
};
#define XTS_CALL __stdcall
#define XTS_EXPORT extern "C" __declspec(dllexport) 

XTS_EXPORT Xts::SeqModel* XTS_CALL XtsSeqModelCreate(void);
XTS_EXPORT Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void);
XTS_EXPORT void XTS_CALL XtsSeqModelDestroy(Xts::SeqModel* seq);
XTS_EXPORT void XTS_CALL XtsSynthModelDestroy(Xts::SynthModel* synth);

XTS_EXPORT Xts::SeqState* XTS_CALL XtsSeqStateCreate(void);
XTS_EXPORT Xts::PlotInput* XTS_CALL XtsPlotInputCreate(void);
XTS_EXPORT Xts::PlotOutput0* XTS_CALL XtsPlotOutputCreate(void);
XTS_EXPORT void XTS_CALL XtsSeqStateDestroy(Xts::SeqState* state);
XTS_EXPORT void XTS_CALL XtsPlotInputDestroy(Xts::PlotInput* input);
XTS_EXPORT void XTS_CALL XtsPlotOutputDestroy(Xts::PlotOutput0* output);

XTS_EXPORT Xts::SeqDSP* XTS_CALL XtsSeqDSPCreate(void);
XTS_EXPORT void XTS_CALL XtsSeqDSPDestroy(Xts::SeqDSP* dsp);
XTS_EXPORT void XTS_CALL XtsSeqDSPInit(Xts::SeqDSP* dsp, Xts::SeqState* state);
XTS_EXPORT void XTS_CALL XtsSeqDSPProcessBuffer(Xts::SeqDSP* dsp, Xts::SeqState* state);

XTS_EXPORT Xts::PlotDSP* XTS_CALL XtsPlotDSPCreate(void);
XTS_EXPORT void XTS_CALL XtsPlotDSPDestroy(Xts::PlotDSP* dsp);
XTS_EXPORT void XTS_CALL XtsPlotDSPRender(Xts::PlotDSP* dsp, Xts::PlotInput const* input, Xts::PlotOutput0* output);

#endif // XTS_HPP