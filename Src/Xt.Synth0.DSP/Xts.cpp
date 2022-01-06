#include "Xts.hpp"
#include "Model/SizeChecks.hpp"

void XTS_CALL XtsDSPInit(void) { Xts::UnitDSP::Init(); }

void XTS_CALL XtsSynthModelDestroy(Xts::SynthModel* synth) { delete synth; }
void XTS_CALL XtsSequencerModelDestroy(Xts::SequencerModel* seq) { delete seq; }
Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void) { return new Xts::SynthModel; }
Xts::SequencerModel* XTS_CALL XtsSequencerModelCreate(void) { return new Xts::SequencerModel; }

void XTS_CALL XtsUnitDSPDestroy(Xts::UnitDSP* dsp) { delete dsp; }
void XTS_CALL XtsUnitDSPReset(Xts::UnitDSP* dsp) { dsp->Reset(); }
Xts::UnitDSP* XTS_CALL XtsUnitDSPCreate(void) { return new Xts::UnitDSP; }
float XTS_CALL XtsUnitDSPFrequency(Xts::UnitDSP* dsp, Xts::UnitModel const* unit) { return dsp->Frequency(*unit); }
float XTS_CALL XtsUnitDSPNext(Xts::UnitDSP* dsp, Xts::UnitModel const* unit, float rate)
{ return dsp->Next(*unit, rate); }

void XTS_CALL XtsSequencerDSPReset(Xts::SequencerDSP* dsp) { dsp->Reset(); }
void XTS_CALL XtsSequencerDSPDestroy(Xts::SequencerDSP* dsp) { delete dsp; }
Xts::SequencerDSP* XTS_CALL XtsSequencerDSPCreate(void) { return new Xts::SequencerDSP; }
void XTS_CALL XtsSequencerDSPProcessBuffer(
  Xts::SequencerDSP* dsp, Xts::SequencerModel const* seq, Xts::SynthModel* synth,
  float rate, float* buffer, int32_t frames, int32_t* currentRow, int64_t* streamPosition)
{ dsp->ProcessBuffer(*seq, *synth, rate, buffer, frames, currentRow, streamPosition); }