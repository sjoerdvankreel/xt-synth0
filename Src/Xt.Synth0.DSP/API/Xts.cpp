#include <API/Xts.hpp>
#include <DSP/Shared/Plot.hpp>
#include <DSP/Synth/PlotDSP.hpp>
#include <DSP/Synth/SynthDSP.hpp>
#include <DSP/Sequencer/SequencerDSP.hpp>
#include <Model/Synth/SynthModel.hpp>
#include <Model/Shared/ParamBinding.hpp>
#include <Model/Shared/SyncStepModel.hpp>
#include <Model/Sequencer/SequencerModel.hpp>

struct XTS_ALIGN XtsPlot
{
  Xts::PlotState state;
  Xts::SynthModel model;
  Xts::ParamBinding binding;
};

struct XTS_ALIGN XtsSequencer
{
  Xts::ParamBinding binding;
  Xts::SynthDSP* synthDsp;
  Xts::SynthModel synthModel;
  Xts::SequencerDSP* sequencerDsp;
  Xts::SequencerModel sequencerModel;
};

void XTS_CALL 
XtsSynthModelInit(Xts::ParamInfo* params, int32_t count)
{ Xts::SynthModel::Init(params, static_cast<size_t>(count)); }

void XTS_CALL 
XtsSyncStepModelInit(Xts::SyncStepModel* steps, int32_t count)
{ Xts::SyncStepModel::Init(steps, static_cast<size_t>(count)); }

Xts::SequencerOutput const* XTS_CALL
XtsSequencerRender(XtsSequencer* sequencer, int32_t frames)
{ return sequencer->sequencerDsp->Render(frames); }

void XTS_CALL
XtsPlotDestroy(XtsPlot* plot)
{
  if (plot == nullptr) return;
  delete plot->state.data;
  delete plot->state.scratch;
  delete plot->binding.params;
  delete plot;
}

void XTS_CALL
XtsSequencerDestroy(XtsSequencer* sequencer)
{
  if (sequencer == nullptr) return;
  delete sequencer->synthDsp;
  delete sequencer->sequencerDsp;
  delete sequencer->binding.params;
  delete sequencer;
}

XtsPlot* XTS_CALL
XtsPlotCreate(int32_t params)
{
  auto result = new XtsPlot;
  result->state.data = new Xts::PlotData;
  result->state.scratch = new Xts::PlotScratch;
  result->binding.params = new int32_t*[params];
  return result;
}

Xts::PlotResult* XTS_CALL
XtsPlotRender(XtsPlot* plot, Xts::PlotInput const* input, Xts::PlotOutput** output)
{
  *plot->state.data = Xts::PlotData();
  plot->state.output = Xts::PlotOutput();
  plot->state.result = Xts::PlotResult();
  *plot->state.scratch = Xts::PlotScratch();
  Xts::SynthPlotRender(plot->model, *input, plot->state);
  *output = &plot->state.output;  
  return &plot->state.result;
}

XtsSequencer* XTS_CALL
XtsSequencerCreate(int32_t params, int32_t frames, float rate)
{
  auto result = new XtsSequencer;
  result->synthDsp = new Xts::SynthDSP();
  result->binding.params = new int32_t * [params];
  result->sequencerDsp = new Xts::SequencerDSP(&result->sequencerModel, rate, frames);
  return result;
}

XTS_EXPORT void XTS_CALL
XtsSequencerConnect(XtsSequencer* sequencer, float rate)
{
  auto const& edit = sequencer->sequencerModel.edit;
  float bpm = static_cast<float>(edit.bpm);
  new(sequencer->synthDsp) Xts::SynthDSP(&sequencer->synthModel, &sequencer->binding, edit.fxs, edit.keys, bpm, rate);
  sequencer->sequencerDsp->Synth(sequencer->synthDsp);
}