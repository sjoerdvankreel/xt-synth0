#include <API/Xts.hpp>
#include <DSP/Synth/PlotDSP.hpp>
#include <DSP/Sequencer/SequencerDSP.hpp>
#include <Model/Synth/SynthModel.hpp>
#include <Model/Shared/ParamBinding.hpp>
#include <Model/Shared/SyncStepModel.hpp>
#include <Model/Sequencer/SequencerModel.hpp>

struct XTS_ALIGN XtsSequencer
{
  float rate;
  int32_t pad__;
  Xts::SequencerDSP* dsp;
  Xts::SynthModel synth;
  Xts::SequencerModel model;
  Xts::ParamBinding binding;
};

void XTS_CALL 
XtsSynthModelInit(Xts::ParamInfo* params, int32_t count)
{ Xts::SynthModel::Init(params, static_cast<size_t>(count)); }

void XTS_CALL 
XtsSyncStepModelInit(Xts::SyncStepModel* steps, int32_t count)
{ Xts::SyncStepModel::Init(steps, static_cast<size_t>(count)); }

Xts::SequencerOutput const* XTS_CALL
XtsSequencerRender(XtsSequencer* sequencer, int32_t frames)
{ return sequencer->dsp->Render(frames, sequencer->rate); }

void XTS_CALL
XtsSequencerDestroy(XtsSequencer* sequencer)
{
  delete sequencer->dsp;
  delete sequencer->binding.params;
  delete sequencer;
}

XtsSequencer* XTS_CALL
XtsSequencerCreate(int32_t params, int32_t frames, float rate)
{
  auto result = new XtsSequencer;
  result->rate = rate;
  result->binding.params = new int32_t*[params];
  result->dsp = new Xts::SequencerDSP(&result->model, &result->synth, &result->binding, frames);
  return result;
}

void XTS_CALL 
XtsPlotStateDestroy(PlotState* state)
{
  delete state->fft;
  delete state->horizontalData;
  delete state->verticalData;
  delete state->leftData;
  delete state->rightData;
  delete state->scratch;
  delete state->horizontalValueData;
  delete state->verticalValueData;
  delete state->horizontalTextData;
  delete state->verticalTextData;
  state->fft = nullptr;
  state->horizontalData = nullptr;
  state->verticalData = nullptr;
  state->leftData = nullptr;
  state->rightData = nullptr;
  state->scratch = nullptr;
  state->horizontalValueData = nullptr;
  state->verticalValueData = nullptr;
  state->horizontalTextData = nullptr;
  state->verticalTextData = nullptr;
  delete state;
}

PlotState* XTS_CALL 
XtsPlotStateCreate(void)
{
  auto result = new PlotState;
  result->leftData = new std::vector<float>();
  result->rightData = new std::vector<float>();
  result->verticalValueData = new std::vector<float>();
  result->horizontalValueData = new std::vector<int32_t>();
  result->verticalData = new std::vector<Xts::VerticalMarker>();
  result->horizontalData = new std::vector<Xts::HorizontalMarker>();
  result->fft = new std::vector<std::complex<float>>();
  result->scratch = new std::vector<std::complex<float>>();
  result->verticalTextData = new std::vector<wchar_t const*>();
  result->horizontalTextData = new std::vector<wchar_t const*>();
  return result;
}

void XTS_CALL 
XtsPlotDSPRender(PlotState* state)
{
  Xts::PlotInput in = {};
  Xts::PlotOutput out = {};

  state->fft->clear();
  state->horizontalData->clear();
  state->verticalData->clear();
  state->scratch->clear();
  state->leftData->clear();
  state->rightData->clear();
  state->horizontalValueData->clear();
  state->verticalValueData->clear();
  state->horizontalTextData->clear();
  state->verticalTextData->clear();

  out.fft = state->fft;
  out.horizontal = state->horizontalData;
  out.vertical = state->verticalData;
  out.left = state->leftData;
  out.right = state->rightData;
  out.scratch = state->scratch;
  in.rate = state->rate;
  in.spectrum = state->spectrum;
  in.bpm = static_cast<float>(state->bpm);
  in.pixels = static_cast<float>(state->pixels);
  Xts::SynthPlotRender(*state->synth, in, out);

  state->min = out.min;
  state->max = out.max;
  state->clip = out.clip;
  state->frequency = out.frequency;
  state->rate = out.rate;
  state->spectrum = out.spectrum;
  state->stereo = out.stereo != 0;
  state->left = state->leftData->data();
  state->right = state->rightData->data();
  state->horizontalCount = static_cast<int32_t>(state->horizontalData->size());
  state->verticalCount = static_cast<int32_t>(state->verticalData->size());
  state->sampleCount = static_cast<int32_t>(state->leftData->size());
  assert(state->sampleCount == state->rightData->size() || state->rightData->size() == 0);
  for(size_t i = 0; i < state->horizontalData->size(); i++)
  {
    state->horizontalValueData->push_back((*state->horizontalData)[i].pos);
    state->horizontalTextData->push_back((*state->horizontalData)[i].text.c_str());    
  }
  for (size_t i = 0; i < state->verticalData->size(); i++)
  {
    state->verticalValueData->push_back((*state->verticalData)[i].pos);
    state->verticalTextData->push_back((*state->verticalData)[i].text.c_str());
  }
  state->horizontalValues = state->horizontalValueData->data();
  state->verticalValues = state->verticalValueData->data();
  state->horizontalTexts = state->horizontalTextData->data();
  state->verticalTexts = state->verticalTextData->data();
}