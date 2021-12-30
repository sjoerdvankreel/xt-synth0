#ifndef XTS_HPP
#define XTS_HPP

#include "DSP/SequencerDSP.hpp"
#include "Model/SynthModel.hpp"
#include "Model/SequencerModel.hpp"

#define XTS_CALL __stdcall
#define XTS_EXPORT extern "C" __declspec(dllexport) 

XTS_EXPORT int XTS_CALL XtsAmpModelSize(void);
XTS_EXPORT int XTS_CALL XtsUnitModelSize(void);
XTS_EXPORT int XTS_CALL XtsSynthModelSize(void);
XTS_EXPORT int XTS_CALL XtsGlobalModelSize(void);

XTS_EXPORT int XTS_CALL XtsEditModelSize(void);
XTS_EXPORT int XTS_CALL XtsPatternFxSize(void);
XTS_EXPORT int XTS_CALL XtsPatternKeySize(void);
XTS_EXPORT int XTS_CALL XtsPatternRowSize(void);
XTS_EXPORT int XTS_CALL XtsPatternModelSize(void);
XTS_EXPORT int XTS_CALL XtsSequencerModelSize(void);

XTS_EXPORT Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void);
XTS_EXPORT void XTS_CALL XtsSynthModelDestroy(Xts::SynthModel* synth);
XTS_EXPORT Xts::SequencerModel* XTS_CALL XtsSequencerModelCreate(void);
XTS_EXPORT void XTS_CALL XtsSequencerModelDestroy(Xts::SequencerModel* seq);

XTS_EXPORT void XTS_CALL XtsDSPReset(Xts::SequencerDSP* dsp);
XTS_EXPORT void XTS_CALL XtsDSPDestroy(Xts::SequencerDSP* dsp);
XTS_EXPORT Xts::SequencerDSP* XTS_CALL XtsDSPCreate(Xts::Param* params, int length);
XTS_EXPORT int XTS_CALL XtsDSPProcessBuffer(Xts::SequencerDSP* dsp, 
  Xts::SequencerModel const* seq, Xts::SynthModel* synth, float rate, float* buffer, int frames);

#endif // XTS_HPP