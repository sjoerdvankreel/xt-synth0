#ifndef XTS_SEQUENCER_MODEL_HPP
#define XTS_SEQUENCER_MODEL_HPP

#include "TrackConstants.hpp"

namespace Xts {

enum class PatternNote { None, Off, C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };

struct PatternFx { int value, target; };
struct PatternKey { int amp, oct, note; };
struct EditModel { int fx, act, pats, keys; };

struct PatternRow
{
  PatternFx fx[TrackConstants::MaxFxCount];
  PatternKey keys[TrackConstants::MaxKeyCount];
};

struct PatternModel
{
  PatternRow rows[TrackConstants::TotalRowCount];
};

struct SequencerModel
{
  EditModel edit;
  PatternModel pattern;
};

} // namespace Xts
#endif // XTS_SEQUENCER_MODEL_HPP