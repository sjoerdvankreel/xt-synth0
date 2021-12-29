#ifndef XTS_H
#define XTS_H

#include "DSP/XtsSequencerDSP.h"
#include "Model/XtsSynthModel.h"
#include "Model/XtsSequencerModel.h"

#define XTS_CALL __stdcall
#define XTS_EXPORT extern "C" __declspec(dllexport)

XTS_EXPORT XtsSequencerDSP* XTS_CALL
XtsSequencerDSPCreate(void);
XTS_EXPORT void XTS_CALL
XtsSequencerDSPDestroy(XtsSequencerDSP* dsp);

XTS_EXPORT XtsSynthModel* XTS_CALL
XtsSynthModelCreate(void);
XTS_EXPORT void XTS_CALL
XtsSynthModelDestroy(XtsSynthModel* synth);

XTS_EXPORT XtsSequencerModel* XTS_CALL
XtsSequencerModelCreate(void);
XTS_EXPORT void XTS_CALL
XtsSequencerModelDestroy(XtsSequencerModel* seq);

XTS_EXPORT int XTS_CALL
XtsProcessBuffer(
XtsSequencerDSP* dsp, const XtsSequencerModel* seq, XtsSynthModel* synth, float rate, float* buffer, int frames);

#endif // XTS_H