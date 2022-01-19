#ifndef XTS_SEQ_MODEL_HPP
#define XTS_SEQ_MODEL_HPP

#include "Model.hpp"
#include <cstdint>

namespace Xts {

struct XTS_ALIGN PatternFx 
{ 
private:
  int tgt, val; 
};
XTS_CHECK_SIZE(PatternFx, 8);

enum class PatternNote { None, Off, C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };
struct XTS_ALIGN PatternKey
{
private:
  PatternNote note;
  int amp, oct, pad__;
};
XTS_CHECK_SIZE(PatternKey, 16);

struct XTS_ALIGN EditModel 
{ 
private:
  int pats, rows, keys, fxs, lpb, edit;
};
XTS_CHECK_SIZE(EditModel, 24);

struct XTS_ALIGN PatternRow
{
private:
  PatternFx fx[MaxFxs];
  PatternKey keys[MaxKeys];
};
XTS_CHECK_SIZE(PatternRow, 88);

struct XTS_ALIGN PatternModel
{
private:
  PatternRow rows[TotalRows];
};
XTS_CHECK_SIZE(PatternModel, 22528);

struct XTS_ALIGN SeqModel
{
private:
  EditModel edit;
  PatternModel pattern;
};
XTS_CHECK_SIZE(SeqModel, 22552);

} // namespace Xts
#endif // XTS_SEQ_MODEL_HPP