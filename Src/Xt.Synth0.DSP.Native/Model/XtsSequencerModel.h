#ifndef XTS_SEQUENCER_MODEL_H
#define XTS_SEQUENCER_MODEL_H

static const int XtsPatternRowMaxFxCount = 3;
static const int XtsPatternRowMaxKeyCount = 4;
static const int XtsPatternModelRowCount = 256;

typedef enum XtsPatternNote
{
	XtsPN_None, XtsPN_Off,
  XtsPN_C, XtsPN_CSharp,
  XtsPN_D, XtsPN_DSharp, XtsPN_E,
  XtsPN_F, XtsPN_FSharp,
  XtsPN_G, XtsPN_GSharp,
  XtsPN_A, XtsPN_ASharp, XtsPN_B
} XtsPatternNote;

typedef struct XtsEditModel
{
  int fx;
  int act;
  int pats;
  int keys;
} XtsEditModel;

typedef struct XtsPatternFx
{
  int value;
  int target;
} XtsPatternFx;

typedef struct XtsPatternKey
{
  int amp;
  int oct;
  XtsPatternNote note;
} XtsPatternKey;

typedef struct XtsPatternRow
{
  XtsPatternFx fx[XtsPatternRowMaxFxCount];
  XtsPatternKey keys[XtsPatternRowMaxKeyCount];
} XtsPatternRow;

typedef struct XtsPatternModel
{
  XtsPatternRow rows[XtsPatternModelRowCount];
} XtsPatternModel;

typedef struct XtsSequencerModel
{
  XtsPatternModel pattern;
  XtsEditModel edit;
} XtsSequencerModel;

#endif // XTS_SEQUENCER_MODEL_H