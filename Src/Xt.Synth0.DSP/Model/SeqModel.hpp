#ifndef XTS_SEQ_MODEL_HPP
#define XTS_SEQ_MODEL_HPP

#include "TrackConstants.hpp"

namespace Xts {

enum class PatternNote { None, Off, C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };

struct XTS_ALIGN PatternFx { int target, value; };
struct XTS_ALIGN PatternKey { int amp, note, oct, pad__; };
struct XTS_ALIGN EditModel { int pats, rows, keys, fxs, lpb, edit; };

struct XTS_ALIGN PatternRow
{
  PatternFx fx[TrackConstants::MaxFxs];
  PatternKey keys[TrackConstants::MaxKeys];
};

struct XTS_ALIGN PatternModel
{
  PatternRow rows[TrackConstants::TotalRows];
};

struct XTS_ALIGN SeqModel
{
  EditModel edit;
  PatternModel pattern;
};

} // namespace Xts
#endif // XTS_SEQ_MODEL_HPP