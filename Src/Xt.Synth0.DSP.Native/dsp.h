#ifndef XTS_DSP_H
#define XTS_DSP_H

#define XTS_CALL __stdcall
#define XTS_EXPORT __declspec(dllexport)

#define XTS_UNIT_TYPE_SIN 0
#define XTS_UNIT_TYPE_SAW 1
#define XTS_UNIT_TYPE_SQR 2
#define XTS_UNIT_TYPE_TRI 3

#define XTS_SYNTH_METHOD_PBP 0
#define XTS_SYNTH_METHOD_ADD 1
#define XTS_SYNTH_METHOD_NVE 2
#define XTS_SYNTH_UNIT_COUNT 3

#define XTS_ROW_FX_COUNT 3
#define XTS_ROW_KEY_COUNT 4
#define XT_PATTERN_ROW_COUNT 256

typedef void (XTS_CALL *
xts_audio_row_updated)(int row);
typedef struct xts_audio { xts_audio_row_updated row_updated; int current_row; } xts_audio;

typedef struct xts_fx { int value, target; } xts_fx;
typedef struct xts_key { int amp, oct, note; } xts_key;
typedef struct xt_edit { int fx, act, pats, keys; } xts_edit;
typedef struct xts_row { xts_fx fx[XTS_ROW_FX_COUNT]; xts_key keys[XTS_ROW_KEY_COUNT]; } xts_row;
typedef struct xts_pattern { xts_row rows[XT_PATTERN_ROW_COUNT]; } xts_pattern;
typedef struct xts_sequencer { xts_edit edit; xts_pattern pattern;} xts_sequencer;

typedef struct xts_amp { int a, d, s, r, lvl; } xts_amp;
typedef struct xts_global { int bpm, plot, hmns, method; } xts_global;
typedef struct xts_unit { int on, amp, oct, note, cent, type; } xts_unit;
typedef struct xts_synth { xts_amp amp; xts_global global; xts_unit units[XTS_SYNTH_UNIT_COUNT]; } xts_synth;

#endif