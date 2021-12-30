#ifndef XTS_SYNTH_DSP_H
#define XTS_SYNTH_DSP_H

#include "XtsUnitDSP.h"
#include "../Model/XtsSynthModel.h"

typedef struct XtsSynthDSP
{
  XtsUnitDSP units[XTS_SYNTH_MODEL_UNIT_COUNT];
} XtsSynthDSP;

extern void
XtsSynthDSPReset(
XtsSynthDSP* dsp);

extern float
XtsSynthDSPNext(
XtsSynthDSP* dsp, const XtsSynthModel* synth, float rate);

#endif // XTS_SYNTH_DSP_H