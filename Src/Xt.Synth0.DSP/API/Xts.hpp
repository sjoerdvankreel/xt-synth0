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

struct XtsSequencer;

namespace Xts {
struct ParamInfo;
struct SynthModel;
struct ParamBinding;
struct SyncStepModel;

class SequencerDSP;
struct SequencerModel;
struct SequencerOutput;
} // namespace Xts

XTS_EXPORT void XTS_CALL
XtsPlotDSPRender(PlotState* state);
XTS_EXPORT PlotState* XTS_CALL
XtsPlotStateCreate(void);
XTS_EXPORT void XTS_CALL
XtsPlotStateDestroy(PlotState* state);

XTS_EXPORT void XTS_CALL
XtsSynthModelInit(Xts::ParamInfo* params, int32_t count);
XTS_EXPORT void XTS_CALL
XtsSyncStepModelInit(Xts::SyncStepModel* steps, int32_t count);

XTS_EXPORT void XTS_CALL
XtsSequencerDestroy(XtsSequencer* sequencer);
XTS_EXPORT Xts::SequencerOutput const* XTS_CALL
XtsSequencerRender(XtsSequencer* sequencer, int32_t frames);
XTS_EXPORT XtsSequencer* XTS_CALL
XtsSequencerCreate(int32_t params, int32_t frames, float rate);

#endif // XTS_API_XTS_HPP