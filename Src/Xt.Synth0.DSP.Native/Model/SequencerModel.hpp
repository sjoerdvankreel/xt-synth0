#ifndef XTS_SEQUENCER_MODEL_HPP
#define XTS_SEQUENCER_MODEL_HPP

namespace Xts {

enum class PatternNote { None, Off, C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };

struct PatternFx { int value, target; };
struct PatternKey { int amp, oct, note; };
struct EditModel { int fx, act, pats, keys; };

struct PatternRow
{
  static constexpr int MaxFxCount = 3;
  static constexpr int MaxKeyCount = 4;
  PatternFx fx[MaxFxCount];
  PatternKey keys[MaxKeyCount];
};

struct PatternModel
{
  static constexpr int PatternRows = 32;
  static constexpr int PatternCount = 8;
  static constexpr int RowCount = PatternCount * PatternRows;
  PatternRow rows[RowCount];
};

struct SequencerModel
{
  EditModel edit;
  PatternModel pattern;
};

} // namespace Xts
#endif // XTS_SEQUENCER_MODEL_HPP