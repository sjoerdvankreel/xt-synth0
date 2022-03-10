#include "Xts.hpp"
#include "DSP/SeqDSP.hpp"
#include "Model/SeqModel.hpp"
#include <DSP/Synth/PlotDSP.hpp>
#include <Model/Synth/SynthModel.hpp>

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
XtsSynthModelInit(Xts::ParamInfo* infos, int32_t infoCount, Xts::SyncStepModel* steps, int32_t stepCount)
{ 
  Xts::SynthModelInit(infos, infoCount);
  Xts::SyncStepModel::Init(steps, static_cast<size_t>(stepCount));
}

void XTS_CALL
XtsSeqDSPInit(Xts::SeqDSP* dsp, Xts::SeqModel const* model, Xts::SynthModel const* synth, Xts::VoiceBinding const* binding)
{ dsp->Init(model, synth, binding); }

void XTS_CALL 
XtsPlotStateDestroy(PlotState* state)
{
  delete state->fftData;
  delete state->hSplitData;
  delete state->vSplitData;
  delete state->lSampleData;
  delete state->rSampleData;
  delete state->fftScratch;
  delete state->hSplitValData;
  delete state->vSplitValData;
  delete state->hSplitMarkerData;
  delete state->vSplitMarkerData;
  state->fftData = nullptr;
  state->hSplitData = nullptr;
  state->vSplitData = nullptr;
  state->lSampleData = nullptr;
  state->rSampleData = nullptr;
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
  result->lSampleData = new std::vector<float>();
  result->rSampleData = new std::vector<float>();
  result->vSplitValData = new std::vector<float>();
  result->hSplitValData = new std::vector<int32_t>();
  result->vSplitData = new std::vector<Xts::VerticalMarker>();
  result->hSplitData = new std::vector<Xts::HorizontalMarker>();
  result->fftData = new std::vector<std::complex<float>>();
  result->fftScratch = new std::vector<std::complex<float>>();
  result->vSplitMarkerData = new std::vector<wchar_t const*>();
  result->hSplitMarkerData = new std::vector<wchar_t const*>();
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
  state->fftScratch->clear();
  state->lSampleData->clear();
  state->rSampleData->clear();
  state->hSplitValData->clear();
  state->vSplitValData->clear();
  state->hSplitMarkerData->clear();
  state->vSplitMarkerData->clear();

  out.fft = state->fftData;
  out.horizontal = state->hSplitData;
  out.vertical = state->vSplitData;
  out.left = state->lSampleData;
  out.right = state->rSampleData;
  out.scratch = state->fftScratch;
  in.rate = state->rate;
  in.spectrum = state->spec;
  in.bpm = static_cast<float>(state->bpm);
  in.pixels = static_cast<float>(state->pixels);
  Xts::SynthPlotRender(*state->synth, in, out);

  state->min = out.min;
  state->max = out.max;
  state->clip = out.clip;
  state->freq = out.frequency;
  state->rate = out.rate;
  state->spec = out.spectrum;
  state->stereo = out.stereo != 0;
  state->lSamples = state->lSampleData->data();
  state->rSamples = state->rSampleData->data();
  state->hSplitCount = static_cast<int32_t>(state->hSplitData->size());
  state->vSplitCount = static_cast<int32_t>(state->vSplitData->size());
  state->sampleCount = static_cast<int32_t>(state->lSampleData->size());
  assert(state->sampleCount == state->rSampleData->size() || state->rSampleData->size() == 0);
  for(size_t i = 0; i < state->hSplitData->size(); i++)
  {
    state->hSplitValData->push_back((*state->hSplitData)[i].pos);
    state->hSplitMarkerData->push_back((*state->hSplitData)[i].text.c_str());    
  }
  for (size_t i = 0; i < state->vSplitData->size(); i++)
  {
    state->vSplitValData->push_back((*state->vSplitData)[i].pos);
    state->vSplitMarkerData->push_back((*state->vSplitData)[i].text.c_str());
  }
  state->hSplitVals = state->hSplitValData->data();
  state->vSplitVals = state->vSplitValData->data();
  state->hSplitMarkers = state->hSplitMarkerData->data();
  state->vSplitMarkers = state->vSplitMarkerData->data();
}