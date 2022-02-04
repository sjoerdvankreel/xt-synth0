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
void XTS_CALL XtsVoiceBindingDestroy(Xts::VoiceBinding* binding) { delete binding; }

SeqState* XTS_CALL XtsSeqStateCreate(void) { return new SeqState; }
Xts::SeqDSP* XTS_CALL XtsSeqDSPCreate(void) { return new Xts::SeqDSP; }
Xts::SeqModel* XTS_CALL XtsSeqModelCreate(void) { return new Xts::SeqModel; }
Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void) { return new Xts::SynthModel; }
Xts::VoiceBinding* XTS_CALL XtsVoiceBindingCreate(void) { return new Xts::VoiceBinding; }

void XTS_CALL 
XtsSynthModelInit(Xts::ParamInfo* infos, int32_t infoCount, Xts::SyncStep* steps, int32_t stepCount)
{ Xts::SynthModelInit(infos, infoCount, steps, stepCount); }
void XTS_CALL
XtsSeqDSPInit(Xts::SeqDSP* dsp, Xts::SeqModel const* model, Xts::SynthModel const* synth, Xts::VoiceBinding const* binding)
{ dsp->Init(model, synth, binding); }

void XTS_CALL 
XtsPlotStateDestroy(PlotState* state)
{
  delete state->fftData;
  delete state->hSplitData;
  delete state->vSplitData;
  delete state->sampleData;
  delete state->fftScratch;
  delete state->hSplitValData;
  delete state->vSplitValData;
  delete state->hSplitMarkerData;
  delete state->vSplitMarkerData;
  state->fftData = nullptr;
  state->hSplitData = nullptr;
  state->vSplitData = nullptr;
  state->sampleData = nullptr;
  state->fftScratch = nullptr;
  state->hSplitValData = nullptr;
  state->vSplitValData = nullptr;
  state->hSplitMarkerData = nullptr;
  state->vSplitMarkerData = nullptr;
  delete state;
}

PlotState* XTS_CALL 
XtsPlotStateCreate(void)
{
  auto result = new PlotState;
  result->sampleData = new std::vector<float>;
  result->vSplitValData = new std::vector<float>;
  result->hSplitValData = new std::vector<int32_t>;
  result->vSplitData = new std::vector<Xts::VSplit>;
  result->hSplitData = new std::vector<Xts::HSplit>;
  result->vSplitMarkerData = new std::vector<char const*>;
  result->hSplitMarkerData = new std::vector<char const*>;
  result->fftData = new std::vector<std::complex<float>>();
  result->fftScratch = new std::vector<std::complex<float>>();
  return result;
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
  state->end = dsp->End();
  state->pos = out.pos;
  state->row = out.row;
  state->clip = out.clip;
  state->voices = out.voices;
  state->exhausted = out.exhausted;
}

void XTS_CALL 
XtsPlotDSPRender(PlotState* state)
{
  Xts::PlotInput in = {};
  Xts::PlotOutput out = {};

  state->fftData->clear();
  state->hSplitData->clear();
  state->vSplitData->clear();
  state->sampleData->clear();
  state->fftScratch->clear();
  state->hSplitValData->clear();
  state->vSplitValData->clear();
  state->hSplitMarkerData->clear();
  state->vSplitMarkerData->clear();

  out.fftData = state->fftData;
  out.hSplits = state->hSplitData;
  out.vSplits = state->vSplitData;
  out.samples = state->sampleData;
  out.fftScratch = state->fftScratch;
  in.rate = state->rate;
  in.bpm = static_cast<float>(state->bpm);
  in.pixels = static_cast<float>(state->pixels);
  Xts::PlotDSP::Render(*state->synth, in, out);

  state->min = out.min;
  state->max = out.max;
  state->clip = out.clip;
  state->freq = out.freq;
  state->rate = out.rate;
  state->samples = state->sampleData->data();
  state->hSplitCount = static_cast<int32_t>(state->hSplitData->size());
  state->vSplitCount = static_cast<int32_t>(state->vSplitData->size());
  state->sampleCount = static_cast<int32_t>(state->sampleData->size());
  for(size_t i = 0; i < state->hSplitData->size(); i++)
  {
    state->hSplitValData->push_back((*state->hSplitData)[i].pos);
    state->hSplitMarkerData->push_back((*state->hSplitData)[i].marker.c_str());    
  }
  for (size_t i = 0; i < state->vSplitData->size(); i++)
  {
    state->vSplitValData->push_back((*state->vSplitData)[i].pos);
    state->vSplitMarkerData->push_back((*state->vSplitData)[i].marker.c_str());
  }
  state->hSplitVals = state->hSplitValData->data();
  state->vSplitVals = state->vSplitValData->data();
  state->hSplitMarkers = state->hSplitMarkerData->data();
  state->vSplitMarkers = state->vSplitMarkerData->data();
}