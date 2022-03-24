#ifndef XTS_API_XTS_HPP
#define XTS_API_XTS_HPP

#include <DSP/Shared/Plot.hpp>
#include <Model/Shared/Model.hpp>

#include <vector>
#include <memory>
#include <cstdint>
#include <complex>

#define XTS_CALL __stdcall
#define XTS_EXPORT extern "C" __declspec(dllexport)

struct XtsPlot;
struct XtsSequencer;

namespace Xts {
struct ParamInfo;
struct PlotInput;
struct PlotOutput;
struct PlotResult;
struct SyncStepModel;
struct SequencerOutput;
struct AutomationAction;
} // namespace Xts

XTS_EXPORT void XTS_CALL
XtsSynthModelInit(Xts::ParamInfo* params, int32_t count);
XTS_EXPORT void XTS_CALL
XtsSyncStepModelInit(Xts::SyncStepModel* steps, int32_t count);

XTS_EXPORT void XTS_CALL
XtsPlotDestroy(XtsPlot* plot);
XTS_EXPORT XtsPlot* XTS_CALL
XtsPlotCreate(int32_t params);
XTS_EXPORT Xts::PlotResult* XTS_CALL
XtsPlotRender(XtsPlot* plot, Xts::PlotInput const* input, Xts::PlotOutput** output);

XTS_EXPORT void XTS_CALL
XtsSequencerInit(XtsSequencer* sequencer);
XTS_EXPORT void XTS_CALL
XtsSequencerDestroy(XtsSequencer* sequencer);
XTS_EXPORT XtsSequencer* XTS_CALL
XtsSequencerCreate(int32_t params, int32_t frames, int32_t keyCount, float bpm, float rate);
XTS_EXPORT Xts::SequencerOutput const* XTS_CALL
XtsSequencerRender(XtsSequencer* sequencer, int32_t frames, Xts::AutomationAction const* actions, int32_t count);

#endif // XTS_API_XTS_HPP