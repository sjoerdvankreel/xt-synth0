#include "Xts.hpp"
#include "Model/SizeChecks.hpp"

void XTS_CALL XtsSeqModelDestroy(Xts::SeqModel* seq) { delete seq; }
void XTS_CALL XtsSynthModelDestroy(Xts::SynthModel* synth) { delete synth; }
Xts::SeqModel* XTS_CALL XtsSeqModelCreate(void) { return new Xts::SeqModel; }
Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void) { return new Xts::SynthModel; }

void XTS_CALL XtsSeqStateDestroy(Xts::SeqState* state) { delete state; }
void XTS_CALL XtsPlotInputDestroy(Xts::PlotInput* input) { delete input; }
void XTS_CALL XtsPlotOutputDestroy(Xts::PlotOutput0* output) { delete output; }
Xts::SeqState* XTS_CALL XtsSeqStateCreate(void) { return new Xts::SeqState; }
Xts::PlotInput* XTS_CALL XtsPlotInputCreate(void) { return new Xts::PlotInput; }
Xts::PlotOutput0* XTS_CALL XtsPlotOutputCreate(void) { return new Xts::PlotOutput0; }

void XTS_CALL XtsSeqDSPDestroy(Xts::SeqDSP* dsp) { delete dsp; }
Xts::SeqDSP* XTS_CALL XtsSeqDSPCreate(void) { return new Xts::SeqDSP; }
void XTS_CALL XtsSeqDSPInit(Xts::SeqDSP* dsp, Xts::SeqState* state) { dsp->Init(*state); }
void XTS_CALL XtsSeqDSPProcessBuffer(Xts::SeqDSP* dsp, Xts::SeqState* state) { dsp->ProcessBuffer(*state); }

XTS_EXPORT void XTS_CALL XtsPlotDSPDestroy(Xts::PlotDSP* dsp) { delete dsp; }
XTS_EXPORT Xts::PlotDSP* XTS_CALL XtsPlotDSPCreate(void) { return new Xts::PlotDSP; }
XTS_EXPORT void XTS_CALL XtsPlotDSPRender(Xts::PlotDSP* dsp, Xts::PlotInput const* input, Xts::PlotOutput0* output) { dsp->Render(*input, *output); }
