#ifndef XTS_UNIT_DSP_H
#define XTS_UNIT_DSP_H

#include "../Model/XtsSynthModel.h"

typedef struct XtsUnitDSP
{
  float phasef;
  double phased;
} XtsUnitDSP;

extern void
XtsUnitDSPReset(
XtsUnitDSP* dsp);

extern float
XtsUnitDSPNext(
XtsUnitDSP* dsp, const XtsGlobalModel* global, const XtsUnitModel* unit, float rate);

#endif // XTS_UNIT_DSP_H