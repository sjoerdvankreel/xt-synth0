#include "Xts.hpp"
#include "Model/SizeChecks.hpp"

void XTS_CALL XtsDSPInit(void) { Xts::UnitDSP::Init(); }

void XTS_CALL XtsSeqModelDestroy(Xts::SeqModel* seq) { delete seq; }
void XTS_CALL XtsSynthModelDestroy(Xts::SynthModel* synth) { delete synth; }
Xts::SeqModel* XTS_CALL XtsSeqModelCreate(void) { return new Xts::SeqModel; }
Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void) { return new Xts::SynthModel; }

void XTS_CALL XtsSeqDSPInit(Xts::SeqDSP* dsp) { dsp->Init(); }
void XTS_CALL XtsSeqDSPDestroy(Xts::SeqDSP* dsp) { delete dsp; }
Xts::SeqDSP* XTS_CALL XtsSeqDSPCreate(void) { return new Xts::SeqDSP; }
void XTS_CALL XtsSeqDSPProcessBuffer(Xts::SeqDSP* dsp, Xts::SeqState* state) { dsp->ProcessBuffer(*state); }

XTS_EXPORT void XTS_CALL XtsPlotDSPDestroy(Xts::PlotDSP* dsp) { delete dsp; }
XTS_EXPORT Xts::PlotDSP* XTS_CALL XtsPlotDSPCreate(void) { return new Xts::PlotDSP; }
XTS_EXPORT void XTS_CALL XtsPlotDSPRender(
  Xts::PlotDSP* dsp, Xts::SynthModel const* synth, int32_t pixels, int32_t* rate, XtsBool* bipolar,
  float* frequency, float** samples, int32_t* sampleCount, int32_t** splits, int32_t* splitCount)
{ dsp->Render(*synth, pixels, rate, bipolar, frequency, samples, sampleCount, splits, splitCount); }