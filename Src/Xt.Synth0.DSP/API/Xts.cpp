#include <API/Xts.hpp>
#include <DSP/Shared/Plot.hpp>
#include <DSP/Synth/SynthDSP.hpp>
#include <DSP/Sequencer/SequencerDSP.hpp>
#include <Model/Synth/SynthModel.hpp>
#include <Model/Shared/SyncStepModel.hpp>
#include <Model/Sequencer/SequencerModel.hpp>

#include <immintrin.h>

struct XTS_ALIGN XtsPlot
{
  Xts::PlotState state;
  Xts::SynthModel model;
  int32_t* binding[XTS_SYNTH_PARAM_COUNT];
};
XTS_CHECK_SIZE(XtsPlot, 2824);

struct XTS_ALIGN XtsSequencer
{
  int32_t** binding;
  int32_t** voiceBindings;
  Xts::SynthDSP* synthDsp;
  Xts::SynthModel* synthModel;
  Xts::SynthModel* voiceModels;
  Xts::SequencerDSP* sequencerDsp;
  Xts::SequencerModel* sequencerModel;
};
XTS_CHECK_SIZE(XtsSequencer, 56);

static void
DisableDenormals()
{
  _MM_SET_FLUSH_ZERO_MODE(_MM_FLUSH_ZERO_ON);
  _MM_SET_DENORMALS_ZERO_MODE(_MM_DENORMALS_ZERO_ON);
}

void XTS_CALL 
XtsSynthModelInit(Xts::ParamInfo* params, int32_t count)
{ Xts::SynthModel::Init(params, static_cast<size_t>(count)); }

void XTS_CALL 
XtsSyncStepModelInit(Xts::SyncStepModel* steps, int32_t count)
{ Xts::SyncStepModel::Init(steps, static_cast<size_t>(count)); }

Xts::SequencerOutput const* XTS_CALL
XtsSequencerRender(XtsSequencer* sequencer, int32_t frames, struct Xts::AutomationAction const* actions, int32_t count)
{ 
  DisableDenormals();
  return sequencer->sequencerDsp->Render(frames, actions, count); 
}

void XTS_CALL
XtsPlotDestroy(XtsPlot* plot)
{
  if (plot == nullptr) return;
  delete plot->state.data;
  delete plot->state.scratch;
  delete plot;
}

void XTS_CALL
XtsSequencerDestroy(XtsSequencer* sequencer)
{
  if (sequencer == nullptr) return;
  delete sequencer->synthDsp;
  delete sequencer->sequencerDsp;
  delete sequencer;
}

XtsPlot* XTS_CALL
XtsPlotCreate(int32_t params)
{
  auto result = new XtsPlot;
  result->state.data = new Xts::PlotData;
  result->state.scratch = new Xts::PlotScratch;
  return result;
}

void XTS_CALL
XtsSequencerInit(XtsSequencer* sequencer)
{
  sequencer->synthDsp->Init();
  sequencer->sequencerDsp->Connect(sequencer->synthDsp);
}

Xts::PlotResult* XTS_CALL
XtsPlotRender(XtsPlot* plot, Xts::PlotInput const* input, Xts::PlotOutput** output)
{
  DisableDenormals();
  *plot->state.data = Xts::PlotData();
  plot->state.output = Xts::PlotOutput();
  plot->state.result = Xts::PlotResult();
  *plot->state.scratch = Xts::PlotScratch();
  Xts::SynthDSP::RenderPlot(plot->model, *input, plot->state);
  *output = &plot->state.output;  
  return &plot->state.result;
}

XtsSequencer* XTS_CALL
XtsSequencerCreate(int32_t params, int32_t frames, int32_t keyCount, float bpm, float rate)
{
  auto result = new XtsSequencer;
  result->synthDsp = new Xts::SynthDSP(keyCount, bpm, rate);
  result->binding = result->synthDsp->Binding();
  result->synthModel = result->synthDsp->Model();
  result->voiceModels = result->synthDsp->VoiceModels();
  result->voiceBindings = result->synthDsp->VoiceBindings();
  result->sequencerDsp = new Xts::SequencerDSP(rate, frames);
  result->sequencerModel = result->sequencerDsp->Model();
  return result;
}