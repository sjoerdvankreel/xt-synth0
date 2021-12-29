#ifndef XTS_SYNTH_MODEL_H
#define XTS_SYNTH_MODEL_H

#define  XTS_SYNTH_MODEL_UNIT_COUNT 3

typedef enum XtsSynthMethod 
{ 
  XtsSmPBP, XtsSmAdd, XtsSmNve 
} XtsSynthMethod;

typedef enum XtsUnitType 
{ 
  XtsUtSin, XtsUtSaw, XtsUtSqr, XtsUtTri 
} XtsUnitType;

typedef enum XtsUnitNote
{
	XtsUnC, XtsUnCSharp,
	XtsUnD, XtsUnDSharp, XtsUnE,
	XtsUnF, XtsUnFSharp,
	XtsUnG, XtsUnGSharp,
	XtsUnA, XtsUnASharp, XtsUnB
} XtsUnitNote;

typedef struct XtsAmpModel
{
  int a;
  int d;
  int s;
  int r;
  int lvl;
} XtsAmpModel;

typedef struct XtsGlobalModel
{
  int bpm;
  int hmns;
  int plot;
  XtsSynthMethod method;
} XtsGlobalModel;

typedef struct XtsUnitModel
{
  int on;
  int amp;
  int oct;
  XtsUnitNote note;
  int cent;
  XtsUnitType type;
} XtsUnitModel;

typedef struct XtsSynthModel
{
  XtsAmpModel amp;
  XtsGlobalModel global;
  XtsUnitModel units[XTS_SYNTH_MODEL_UNIT_COUNT];
} XtsSynthModel;

#endif // XTS_SYNTH_MODEL_H