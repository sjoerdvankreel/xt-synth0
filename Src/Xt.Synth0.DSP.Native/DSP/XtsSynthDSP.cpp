#include "XtsSynthDSP.h"

void
XtsSynthDSPReset(
XtsSynthDSP* dsp)
{
  for(int u = 0; u < XTS_SYNTH_MODEL_UNIT_COUNT; u++)
    XtsUnitDSPReset(&dsp->units[u]);
}

float
XtsSynthDSPNext(
XtsSynthDSP* dsp, const XtsSynthModel* synth, float rate)
{
  return 0.0f;
}