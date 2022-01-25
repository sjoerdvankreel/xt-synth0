#ifndef XTS_SEQ_MODEL_HPP
#define XTS_SEQ_MODEL_HPP

#include "Model.hpp"
#include <cstdint>

namespace Xts {

struct XTS_ALIGN Param 
{
  friend class SeqDSP;
  Param() = default;
  Param(Param const&) = delete;
private:
  int32_t* val;
  int32_t min, max;
};
XTS_CHECK_SIZE(Param, 16);

struct XTS_ALIGN PatternFx 
{
  friend class SeqDSP;
  PatternFx() = default;
  PatternFx(PatternFx const&) = delete;
private:
  int tgt, val; 
};
XTS_CHECK_SIZE(PatternFx, 8);

enum class PatternNote { None, Off, C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };
struct XTS_ALIGN PatternKey
{
  friend class SeqDSP;
  PatternKey() = default;
  PatternKey(PatternKey const&) = delete;
private:
  PatternNote note;
  int amp, oct, pad__;
};
XTS_CHECK_SIZE(PatternKey, 16);

struct XTS_ALIGN PatternRow
{
  friend class SeqDSP;
  PatternRow() = default;
  PatternRow(PatternRow const&) = delete;
private:
  PatternFx fx[MaxFxs];
  PatternKey keys[MaxKeys];
};
XTS_CHECK_SIZE(PatternRow, 88);

struct XTS_ALIGN PatternModel
{
  friend class SeqDSP;
  PatternModel() = default;
  PatternModel(PatternRow const&) = delete;
private:
  PatternRow rows[TotalRows];
};
XTS_CHECK_SIZE(PatternModel, 22528);

struct XTS_ALIGN EditModel
{
  friend class SeqDSP;
  EditModel() = default;
  EditModel(EditModel const&) = delete;
private:
  int pats, rows, keys, fxs, lpb, edit, bpm, pad__;
};
XTS_CHECK_SIZE(EditModel, 32);

struct XTS_ALIGN SeqModel
{
  friend class SeqDSP;
  SeqModel() = default;
  SeqModel(SeqModel const&) = delete;
private:
  EditModel edit;
  PatternModel pattern;
  Param params[ParamCount];
};
XTS_CHECK_SIZE(SeqModel, 24672);

} // namespace Xts
#endif // XTS_SEQ_MODEL_HPP