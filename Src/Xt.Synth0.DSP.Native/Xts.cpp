#include "Xts.hpp"
#include <stdlib.h>

XTS_EXPORT int XTS_CALL XtsAmpModelSize(void) { return sizeof(Xts::AmpModel); }
XTS_EXPORT int XTS_CALL XtsUnitModelSize(void) { return sizeof(Xts::UnitModel); }
XTS_EXPORT int XTS_CALL XtsSynthModelSize(void) { return sizeof(Xts::SynthModel); }
XTS_EXPORT int XTS_CALL XtsGlobalModelSize(void) { return sizeof(Xts::GlobalModel); }

XTS_EXPORT int XTS_CALL XtsEditModelSize(void) { return sizeof(Xts::EditModel); }
XTS_EXPORT int XTS_CALL XtsPatternFxSize(void) { return sizeof(Xts::PatternFx); }
XTS_EXPORT int XTS_CALL XtsPatternKeySize(void) { return sizeof(Xts::PatternKey); }
XTS_EXPORT int XTS_CALL XtsPatternRowSize(void) { return sizeof(Xts::PatternRow); }
XTS_EXPORT int XTS_CALL XtsPatternModelSize(void) { return sizeof(Xts::PatternModel); }
XTS_EXPORT int XTS_CALL XtsSequencerModelSize(void) { return sizeof(Xts::SequencerModel); }

XTS_EXPORT void XTS_CALL XtsDSPReset(Xts::SequencerDSP* dsp) { dsp->Reset(); }
XTS_EXPORT void XTS_CALL XtsDSPDestroy(Xts::SequencerDSP* dsp) { delete dsp; }
XTS_EXPORT Xts::SequencerDSP* XTS_CALL XtsDSPCreate(Xts::Param* params, int length)
{ return new Xts::SequencerDSP(std::vector<Xts::Param>(params, params + length)); }

XTS_EXPORT void XTS_CALL XtsSynthModelDestroy(Xts::SynthModel* synth) { delete synth; }
XTS_EXPORT void XTS_CALL XtsSequencerModelDestroy(Xts::SequencerModel* seq) { delete seq; }
XTS_EXPORT Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void) { return new Xts::SynthModel; }
XTS_EXPORT Xts::SequencerModel* XTS_CALL XtsSequencerModelCreate(void) { return new Xts::SequencerModel; }

XTS_EXPORT int XTS_CALL XtsDSPProcessBuffer(Xts::SequencerDSP* dsp,
  Xts::SequencerModel const* seq, Xts::SynthModel* synth, float rate, float* buffer, int frames)
{
  for(int f = 0; f < frames; f++)
  {
    float sample = dsp->Next(*seq, *synth, rate);
    buffer[f * 2] = sample;
    buffer[f * 2 + 1] = sample;
  }
  return dsp->CurrentRow();
}