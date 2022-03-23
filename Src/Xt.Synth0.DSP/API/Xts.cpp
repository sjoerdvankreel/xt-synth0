#include <API/Xts.hpp>
#include <DSP/Shared/Plot.hpp>
#include <DSP/Synth/PlotDSP.hpp>
#include <DSP/Synth/SynthDSP.hpp>
#include <DSP/Sequencer/SequencerDSP.hpp>
#include <Model/Synth/SynthModel.hpp>
#include <Model/Shared/SyncStepModel.hpp>
#include <Model/Sequencer/SequencerModel.hpp>

struct XTS_ALIGN XtsPlot
{
  int32_t** binding;
  Xts::SynthDSP* dsp;
  Xts::PlotState state;
  Xts::SynthModel* model;
};

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

void XTS_CALL 
XtsSynthModelInit(Xts::ParamInfo* params, int32_t count)
{ Xts::SynthModel::Init(params, static_cast<size_t>(count)); }

void XTS_CALL 
XtsSyncStepModelInit(Xts::SyncStepModel* steps, int32_t count)
{ Xts::SyncStepModel::Init(steps, static_cast<size_t>(count)); }

Xts::SequencerOutput const* XTS_CALL
XtsSequencerRender(XtsSequencer* sequencer, int32_t frames, struct Xts::AutomationAction const* actions, int32_t count)
{ return sequencer->sequencerDsp->Render(frames, actions, count); }

void XTS_CALL
XtsPlotInit(XtsPlot* plot, float bpm, float rate)
{ 
  new(plot->dsp) Xts::SynthDSP(0, 1, bpm, rate);
  plot->dsp->Init(); 
}

void XTS_CALL
XtsPlotDestroy(XtsPlot* plot)
{
  if (plot == nullptr) return;
  delete plot->dsp;
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
XtsPlotCreate(void)
{
  auto result = new XtsPlot;
  result->dsp = new Xts::SynthDSP;
  result->state.data = new Xts::PlotData;
  result->state.scratch = new Xts::PlotScratch;
  result->model = result->dsp->Model();
  result->binding = result->dsp->Binding();
  return result;
}

Xts::PlotResult* XTS_CALL
XtsPlotRender(XtsPlot* plot, Xts::PlotInput const* input, Xts::PlotOutput** output)
{
  *plot->state.data = Xts::PlotData();
  plot->state.output = Xts::PlotOutput();
  plot->state.result = Xts::PlotResult();
  *plot->state.scratch = Xts::PlotScratch();
  Xts::SynthPlotRender(*plot->dsp, *input, plot->state);
  *output = &plot->state.output;  
  return &plot->state.result;
}

XtsSequencer* XTS_CALL
XtsSequencerCreate(int32_t params, int32_t frames, float rate)
{
  auto result = new XtsSequencer;
  result->synthDsp = new Xts::SynthDSP();
  result->sequencerDsp = new Xts::SequencerDSP(rate, frames);
  result->binding = result->synthDsp->Binding();
  result->synthModel = result->synthDsp->Model();
  result->voiceModels = result->synthDsp->VoiceModels();
  result->sequencerModel = result->sequencerDsp->Model();
  result->voiceBindings = result->synthDsp->VoiceBindings();
  return result;
}

void XTS_CALL
XtsSequencerConnect(XtsSequencer* sequencer, float rate)
{
  auto const& edit = sequencer->sequencerDsp->Model()->edit;
  float bpm = static_cast<float>(edit.bpm);
  new(sequencer->synthDsp) Xts::SynthDSP(edit.fxs, edit.keys, bpm, rate);
  sequencer->sequencerDsp->Connect(sequencer->synthDsp);
}