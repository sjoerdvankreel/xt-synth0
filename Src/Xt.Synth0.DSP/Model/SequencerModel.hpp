#ifndef XTS_SEQUENCER_MODEL_HPP
#define XTS_SEQUENCER_MODEL_HPP

#include "TrackConstants.hpp"

namespace Xts {

enum class PatternNote { None, Off, C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };

struct XTS_ALIGN PatternFx { int target, value; };
struct XTS_ALIGN PatternKey { int amp, note, octave, pad__; };
struct XTS_ALIGN EditModel { int keyCount, fxCount, patternCount, activePattern; };

struct XTS_ALIGN PatternRow
{
  PatternFx fx[TrackConstants::MaxFxCount];
  PatternKey keys[TrackConstants::MaxKeyCount];
};

struct XTS_ALIGN PatternModel
{
  PatternRow rows[TrackConstants::TotalRowCount];
};

struct XTS_ALIGN SequencerModel
{
  EditModel edit;
  PatternModel pattern;
};

} // namespace Xts
#endif // XTS_SEQUENCER_MODEL_HPP