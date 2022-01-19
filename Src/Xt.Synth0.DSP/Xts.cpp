#include "Xts.hpp"
#include "DSP/SeqDSP.hpp"
#include "Model/DSPModel.hpp"
#include "Model/SeqModel.hpp"
#include "Model/SynthModel.hpp"

void XTS_CALL XtsSeqDSPInit(Xts::SeqDSP* dsp) { dsp->Init(); }
void XTS_CALL XtsSeqDSPDestroy(Xts::SeqDSP* dsp) { delete dsp; }
Xts::SeqDSP* XTS_CALL XtsSeqDSPCreate(void) { return new Xts::SeqDSP; }

void XTS_CALL XtsSeqStateDestroy(SeqState* state) { delete state; }
void XTS_CALL XtsPlotStateDestroy(PlotState* state) { delete state; }
void XTS_CALL XtsSeqModelDestroy(Xts::SeqModel* model) { delete model; }
void XTS_CALL XtsSynthModelDestroy(Xts::SynthModel* model) { delete model; }

SeqState* XTS_CALL XtsSeqStateCreate(void) { return new SeqState; }
PlotState* XTS_CALL XtsPlotStateCreate(void) { return new PlotState; }
Xts::SeqModel* XTS_CALL XtsSeqModelCreate(void) { return new Xts::SeqModel; }
Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void) { return new Xts::SynthModel; }

void XTS_CALL 
XtsSeqDSPRender(Xts::SeqDSP* dsp, SeqState* state)
{
  Xts::SeqInput in;
  in.seq = state->seq;
  in.rate = state->rate;
  in.synth = state->synth;
  in.buffer = state->buffer;
  in.frames = state->frames;

  Xts::SeqOutput out;
  dsp->Render(in, out);
  state->pos = out.pos;
  state->row = out.row;
  state->clip = out.clip;
  state->voices = out.voices;
  state->exhausted = out.exhausted;
}

void XTS_CALL 
XtsPlotDSPRender(Xts::PlotDSP* dsp, PlotState* state)
{
  Xts::PlotInput in;
  in.bpm = state->bpm;
  in.pixels = state->pixels;

  Xts::PlotOutput out;
  state->splits->clear();
  state->samples->clear();
  out.splits = state->splits.get();
  out.samples = state->samples.get();
  dsp->Render(in, out);  
 // out.
}
