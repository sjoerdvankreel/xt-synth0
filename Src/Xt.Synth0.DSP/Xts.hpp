#ifndef XTS_HPP
#define XTS_HPP

#include "DSP/SequencerDSP.hpp"
#include "Model/SynthModel.hpp"
#include "Model/SequencerModel.hpp"
#include <cstdint>

#define XTS_CALL __stdcall
#define XTS_EXPORT extern "C" __declspec(dllexport) 

XTS_EXPORT void XTS_CALL XtsDSPInit(void);

XTS_EXPORT Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void);
XTS_EXPORT void XTS_CALL XtsSynthModelDestroy(Xts::SynthModel* synth);
XTS_EXPORT Xts::SequencerModel* XTS_CALL XtsSequencerModelCreate(void);
XTS_EXPORT void XTS_CALL XtsSequencerModelDestroy(Xts::SequencerModel* seq);

XTS_EXPORT Xts::UnitDSP* XTS_CALL XtsUnitDSPCreate(void);
XTS_EXPORT void XTS_CALL XtsUnitDSPReset(Xts::UnitDSP* dsp);
XTS_EXPORT void XTS_CALL XtsUnitDSPDestroy(Xts::UnitDSP* dsp);
XTS_EXPORT float XTS_CALL XtsUnitDSPFrequency(Xts::UnitDSP* dsp, Xts::UnitModel const* unit);
XTS_EXPORT float XTS_CALL XtsUnitDSPNext(Xts::UnitDSP* dsp, Xts::GlobalModel const* global, Xts::UnitModel const* unit, float rate);

XTS_EXPORT Xts::SequencerDSP* XTS_CALL XtsSequencerDSPCreate(void);
XTS_EXPORT void XTS_CALL XtsSequencerDSPReset(Xts::SequencerDSP* dsp);
XTS_EXPORT void XTS_CALL XtsSequencerDSPDestroy(Xts::SequencerDSP* dsp);
XTS_EXPORT void XTS_CALL XtsSequencerDSPDSPProcessBuffer(
  Xts::SequencerDSP* dsp, Xts::SequencerModel const* seq, Xts::SynthModel* synth, 
  float rate, float* buffer, int32_t frames, int32_t* currentRow, uint64_t* streamPosition);

#endif // XTS_HPP