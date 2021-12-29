#ifndef XTS_SEQUENCER_DSP_H
#define XTS_SEQUENCER_DSP_H

#include "XtsSynthDSP.h"
#include "../Model/XtsSynthModel.h"
#include "../Model/XtsSequencerModel.h"

typedef struct XtsSequencerDSP
{
  int currentRow;
	int previousRow;
	double rowFactor;
  XtsSynthDSP synth;
} XtsSequencerDSP;

extern void
XtsSequencerDSPReset(
XtsSequencerDSP* dsp);

extern float
XtsSequencerDSPNext(
XtsSequencerDSP* dsp, const XtsSequencerModel* seq, XtsSynthModel* synth, float rate);

#endif // XTS_SEQUENCER_DSP_H