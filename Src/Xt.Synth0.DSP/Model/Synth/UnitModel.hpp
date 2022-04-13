#ifndef XTS_MODEL_SYNTH_UNIT_MODEL_HPP
#define XTS_MODEL_SYNTH_UNIT_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Shared/NoteType.hpp>
#include <Model/Synth/TargetModsModel.hpp>

namespace Xts {

enum class PMType
{
  SnSn, SnS2, Sn2S, SnSw, SnSq, 
  S2Sn, S2S2, S22S, S2Sw, S2Sq,
  T2SSn, T2SS2, T2S2S, T2SSw, T2SSq,
  SwSn, SwS2, Sw2S, SwSw, SwSq, 
  SqSn, SqS2, Sq2S, SqSw, SqSq,
};

enum class BlepType { Saw, Pulse, Triangle };
enum class UnitType { Sine, Additive, PolyBlep, PM, PMD };

enum class UnitModTarget 
{ 
  Amp, 
  Pan,
  Phase, 
  Pitch, 
  Frequency,
  PMIndex,
  PMDamping,
  BlepPulseWidth,
  AdditiveRolloff 
};

struct XTS_ALIGN UnitModel
{
  XtsBool on;
  UnitType type;
  int32_t amp;
  int32_t pan;

  NoteType note;
  int32_t octave;
  int32_t detune;
  int32_t pad__;

  BlepType blepType;
  int32_t blepPulseWidth;

  XtsBool additiveSub;
  int32_t additiveStep;
  int32_t additiveRolloff;
  int32_t additivePartials;

  PMType pmType;
  int32_t pmIndex;
  int32_t pmAmount;
  int32_t pmDamping;

  TargetModsModel mods;
};
XTS_CHECK_SIZE(UnitModel, 104);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_UNIT_MODEL_HPP