#ifndef XTS_MODEL_SYNTH_UNIT_MODEL_HPP
#define XTS_MODEL_SYNTH_UNIT_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Shared/NoteType.hpp>
#include <Model/Synth/TargetModsModel.hpp>

namespace Xts {

enum class BlepType { Saw, Pulse, Triangle };
enum class UnitType { Sine, Additive, PolyBlep, PM, PMD };
enum class PMType { Sin, Sn2, Sn3, T2Sn, T3Sn, Saw, SawD, Sqr, SqrD };

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

  int32_t pmIndex;
  int32_t pmDamping;
  PMType pmCarrier;
  PMType pmModulator;

  TargetModsModel mods;
};
XTS_CHECK_SIZE(UnitModel, 104);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_UNIT_MODEL_HPP