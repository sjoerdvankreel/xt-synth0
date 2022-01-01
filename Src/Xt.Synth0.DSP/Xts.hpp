#ifndef XTS_HPP
#define XTS_HPP

#include "DSP/SequencerDSP.hpp"
#include "Model/SynthModel.hpp"
#include "Model/SequencerModel.hpp"
#include <cstdint>

#define XTS_CALL __stdcall
#define XTS_EXPORT extern "C" __declspec(dllexport) 

XTS_EXPORT void XTS_CALL XtsDSPInit(void);
XTS_EXPORT Xts::SequencerDSP* XTS_CALL XtsDSPCreate(void);
XTS_EXPORT void XTS_CALL XtsDSPReset(Xts::SequencerDSP* dsp);
XTS_EXPORT void XTS_CALL XtsDSPDestroy(Xts::SequencerDSP* dsp);

XTS_EXPORT Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void);
XTS_EXPORT void XTS_CALL XtsSynthModelDestroy(Xts::SynthModel* synth);
XTS_EXPORT Xts::SequencerModel* XTS_CALL XtsSequencerModelCreate(void);
XTS_EXPORT void XTS_CALL XtsSequencerModelDestroy(Xts::SequencerModel* seq);

XTS_EXPORT void XTS_CALL
XtsDSPProcessBuffer(
  Xts::SequencerDSP* dsp, 
  Xts::SequencerModel const* seq, 
  Xts::SynthModel* synth, 
  float rate, float* buffer, int32_t frames,
  int32_t* currentRow, uint64_t* streamPosition);

#endif // XTS_HPP