#ifndef XTS_SYNTH_MODEL_H
#define XTS_SYNTH_MODEL_H

static const int XtsSynthModelUnitCount = 3;

typedef enum XtsSynthMethod 
{ 
  XtsSM_PBP, XtsSM_Add, XtsSM_Nve 
} XtsSynthMethod;

typedef enum XtsUnitType 
{ 
  XtsUT_Sin, XtsUT_Saw, XtsUT_Sqr, XtsUT_Tri 
} XtsUnitType;

typedef enum XtsUnitNote
{
	XtsUN_C, XtsUN_CSharp,
	XtsUN_D, XtsUN_DSharp, XtsUN_E,
	XtsUN_F, XtsUN_FSharp,
	XtsUN_G, XtsUN_GSharp,
	XtsUN_A, XtsUN_ASharp, XtsUN_B
} XtsUnitNote;

typedef struct XtsAmpModel
{
  int a;
  int d;
  int s;
  int r;
  int lvl;
};

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
  XtsUnitModel units[XtsSynthModelUnitCount];
} XtsSynthModel;

#endif // XTS_SYNTH_MODEL_H