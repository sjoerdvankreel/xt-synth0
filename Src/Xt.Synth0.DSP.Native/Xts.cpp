#include "Xts.hpp"
#include "Model/SizeChecks.hpp"

void XTS_CALL XtsDSPInit(void) { Xts::UnitDSP::Init(); }
void XTS_CALL XtsDSPReset(Xts::SequencerDSP* dsp) { dsp->Reset(); }
void XTS_CALL XtsDSPDestroy(Xts::SequencerDSP* dsp) { delete dsp; }
Xts::SequencerDSP* XTS_CALL XtsDSPCreate(void) { return new Xts::SequencerDSP; }

void XTS_CALL XtsSynthModelDestroy(Xts::SynthModel* synth) { delete synth; }
void XTS_CALL XtsSequencerModelDestroy(Xts::SequencerModel* seq) { delete seq; }
Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void) { return new Xts::SynthModel; }
Xts::SequencerModel* XTS_CALL XtsSequencerModelCreate(void) { return new Xts::SequencerModel; }

int XTS_CALL
XtsDSPProcessBuffer(
  Xts::SequencerDSP* dsp,
  Xts::SequencerModel const* seq,
  Xts::SynthModel* synth,
  float rate, float* buffer, int frames)
{
  for(int f = 0; f < frames; f++)
  {
    float sample = dsp->Next(*seq, *synth, rate);
    buffer[f * 2] = sample;
    buffer[f * 2 + 1] = sample;
  }
  return dsp->CurrentRow();
}