#include "Xts.hpp"
#include "DSP/SeqDSP.hpp"
#include "DSP/PlotDSP.hpp"
#include "Model/DSPModel.hpp"
#include "Model/SeqModel.hpp"
#include "Model/SynthModel.hpp"

void XTS_CALL XtsSeqDSPDestroy(Xts::SeqDSP* dsp) { delete dsp; }
void XTS_CALL XtsSeqStateDestroy(SeqState* state) { delete state; }
void XTS_CALL XtsSeqModelDestroy(Xts::SeqModel* model) { delete model; }
void XTS_CALL XtsSynthModelDestroy(Xts::SynthModel* model) { delete model; }

SeqState* XTS_CALL XtsSeqStateCreate(void) { return new SeqState; }
Xts::SeqDSP* XTS_CALL XtsSeqDSPCreate(void) { return new Xts::SeqDSP; }
Xts::SeqModel* XTS_CALL XtsSeqModelCreate(void) { return new Xts::SeqModel; }
Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void) { return new Xts::SynthModel; }

PlotState* XTS_CALL 
XtsPlotStateCreate(void)
{
  auto result = new PlotState;
  result->sampleData = new std::vector<float>;
  result->splitData = new std::vector<int32_t>;
  return result;
}

void XTS_CALL 
XtsPlotStateDestroy(PlotState* state)
{
  delete state->splitData;
  delete state->sampleData;
  state->splitData = nullptr;
  state->sampleData = nullptr;
  delete state;
}

void XTS_CALL
XtsSeqDSPRender(Xts::SeqDSP* dsp, SeqState* state)
{
  Xts::SeqInput in;
  in.buffer = state->buffer;
  in.frames = state->frames;
  in.rate = static_cast<float>(state->rate);

  Xts::SeqOutput out;
  dsp->Render(in, out);
  state->pos = out.pos;
  state->row = out.row;
  state->clip = out.clip;
  state->voices = out.voices;
  state->exhausted = out.exhausted;
}

void XTS_CALL 
XtsPlotDSPRender(PlotState* state)
{
  Xts::PlotInput in;
  in.bpm = static_cast<float>(state->bpm);
  in.pixels = static_cast<float>(state->pixels);

  Xts::PlotOutput out;
  state->splitData->clear();
  state->sampleData->clear();
  out.splits = state->splitData;
  out.samples = state->sampleData;
  Xts::PlotDSP::Render(*state->synth, in, out);
  state->splits = state->splitData->data();
  state->samples = state->sampleData->data();
  state->splitCount = static_cast<int32_t>(state->splitData->size());
  state->sampleCount = static_cast<int32_t>(state->sampleData->size());
}

void XTS_CALL
XtsSeqDSPInit(Xts::SeqDSP* dsp, Xts::SeqModel const* model, Xts::SynthModel const* synth)
{
  dsp->Init(model, synth);
}