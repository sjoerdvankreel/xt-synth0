#include "XtsUnitDSP.h"

void
XtsUnitDSPReset(
XtsUnitDSP* dsp)
{
  dsp->phased = 0.0;
  dsp->phasef = 0.0f;
}

float
XtsUnitDSPNext(
XtsUnitDSP* dsp, const XtsGlobalModel* global, const XtsUnitModel* unit, float rate)
{
  return 0.0f;
}