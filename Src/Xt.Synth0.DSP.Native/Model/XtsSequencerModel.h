#ifndef XTS_SEQUENCER_MODEL_H
#define XTS_SEQUENCER_MODEL_H

#define XTS_PATTERN_ROW_MAX_FX_COUNT 3
#define XTS_PATTERN_ROW_MAX_KEY_COUNT 4
#define XTS_PATTERN_MODEL_ROW_COUNT 256

typedef enum XtsPatternNote
{
	XtsPnNone, XtsPnOff,
  XtsPnC, XtsPnCSharp,
  XtsPnD, XtsPnDSharp, XtsPnE,
  XtsPnF, XtsPnFSharp,
  XtsPnG, XtsPnGSharp,
  XtsPnA, XtsPnASharp, XtsPnB
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
  XtsPatternFx fx[XTS_PATTERN_ROW_MAX_FX_COUNT];
  XtsPatternKey keys[XTS_PATTERN_ROW_MAX_KEY_COUNT];
} XtsPatternRow;

typedef struct XtsPatternModel
{
  XtsPatternRow rows[XTS_PATTERN_MODEL_ROW_COUNT];
} XtsPatternModel;

typedef struct XtsSequencerModel
{
  XtsPatternModel pattern;
  XtsEditModel edit;
} XtsSequencerModel;

#endif // XTS_SEQUENCER_MODEL_H