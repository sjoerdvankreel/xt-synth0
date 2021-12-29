#include "XtsSequencerDSP.h"

void
XtsSequencerDSPReset(
XtsSequencerDSP* dsp)
{
  dsp->currentRow = 0;
  dsp->previousRow = -1;
	dsp->rowFactor = 0.0f;
  XtsSynthDSPReset(&dsp->synth);
}

float
XtsSequencerDSPNext(
XtsSequencerDSP* dsp, const XtsSequencerModel* seq, XtsSynthModel* synth, float rate)
{ 
  return 0.0f;
}