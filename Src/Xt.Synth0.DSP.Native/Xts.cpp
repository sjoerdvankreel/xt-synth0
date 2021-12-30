#include "Xts.h"
#include <stdlib.h>

XTS_EXPORT void XTS_CALL
XtsDSPReset(
XtsSequencerDSP* dsp)
{ XtsSequencerDSPReset(dsp); }

void XTS_CALL
XtsSynthModelDestroy(XtsSynthModel* synth)
{ free(synth); }
XtsSynthModel* XTS_CALL
XtsSynthModelCreate(void)
{ return calloc(1, sizeof(XtsSynthModel)); }

void XTS_CALL
XtsDSPDestroy(XtsSequencerDSP* dsp)
{ free(dsp); }
XtsSequencerDSP* XTS_CALL
XtDSPCreate(void)
{ return calloc(1, sizeof(XtsSequencerDSP)); }

XtsSequencerModel* XTS_CALL
XtsSequencerModelCreate(void)
{ return calloc(1, sizeof(XtsSequencerModel)); }
void XTS_CALL
XtsSequencerModelDestroy(XtsSequencerModel* seq)
{ free(seq); }

int XTS_CALL
XtsProcessBuffer(
XtsSequencerDSP* dsp, const XtsSequencerModel* seq, XtsSynthModel* synth, float rate, float* buffer, int frames)
{
  for(int f = 0; f < frames; f++)
  {
    float sample = XtsSequencerDSPNext(dsp, seq, synth, rate);
    buffer[f * 2] = sample;
    buffer[f * 2 + 1] = sample;
    dsp->streamPosition++;
  }
  return dsp->currentRow;
}