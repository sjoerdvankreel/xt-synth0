#include "Xts.hpp"
#include "Model/SizeChecks.hpp"

void XTS_CALL XtsDSPInit(void) { Xts::UnitDSP::Init(); }

void XTS_CALL XtsSynthModelDestroy(Xts::SynthModel* synth) { delete synth; }
void XTS_CALL XtsSequencerModelDestroy(Xts::SequencerModel* seq) { delete seq; }
Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void) { return new Xts::SynthModel; }
Xts::SequencerModel* XTS_CALL XtsSequencerModelCreate(void) { return new Xts::SequencerModel; }

void XTS_CALL XtsSequencerDSPReset(Xts::SequencerDSP* dsp) { dsp->Reset(); }
void XTS_CALL XtsSequencerDSPDestroy(Xts::SequencerDSP* dsp) { delete dsp; }
Xts::SequencerDSP* XTS_CALL XtsSequencerDSPCreate(void) { return new Xts::SequencerDSP; }
void XTS_CALL XtsSequencerDSPProcessBuffer(
  Xts::SequencerDSP* dsp, Xts::SequencerModel const* seq, Xts::SynthModel* synth,
  float rate, float* buffer, int32_t frames, int32_t* currentRow, int64_t* streamPosition)
{ dsp->ProcessBuffer(*seq, *synth, rate, buffer, frames, currentRow, streamPosition); }

XTS_EXPORT void XTS_CALL XtsPlotDSPDestroy(Xts::PlotDSP* dsp) { delete dsp; }
XTS_EXPORT Xts::PlotDSP* XTS_CALL XtsPlotDSPCreate(void) { return new Xts::PlotDSP; }
XTS_EXPORT void XTS_CALL XtsPlotDSPRender(
  Xts::PlotDSP* dsp, Xts::SynthModel const* synth, int32_t pixels, int32_t* rate, 
  float* frequency, float** samples, int32_t* sampleCount, int32_t** splits, int32_t* splitCount)
{ dsp->Render(*synth, pixels, rate, frequency, samples, sampleCount, splits, splitCount); }