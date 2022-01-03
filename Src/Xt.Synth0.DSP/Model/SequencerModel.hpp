#ifndef XTS_SEQUENCER_MODEL_HPP
#define XTS_SEQUENCER_MODEL_HPP

#include "TrackConstants.hpp"

namespace Xts {

enum class PatternNote { None, Off, C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };

struct alignas(TrackConstants::Alignment) PatternFx { int value, target; };
struct alignas(TrackConstants::Alignment) PatternKey { int amp, oct, note; };
struct alignas(TrackConstants::Alignment) EditModel { int fx, act, pats, keys; };

struct alignas(TrackConstants::Alignment) PatternRow
{
  PatternFx fx[TrackConstants::MaxFxCount];
  PatternKey keys[TrackConstants::MaxKeyCount];
};

struct alignas(TrackConstants::Alignment) PatternModel
{
  PatternRow rows[TrackConstants::TotalRowCount];
};

struct alignas(TrackConstants::Alignment) SequencerModel
{
  EditModel edit;
  PatternModel pattern;
};

} // namespace Xts
#endif // XTS_SEQUENCER_MODEL_HPP