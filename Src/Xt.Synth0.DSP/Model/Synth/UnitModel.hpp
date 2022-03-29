#ifndef XTS_MODEL_SYNTH_UNIT_MODEL_HPP
#define XTS_MODEL_SYNTH_UNIT_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Synth/TargetModsModel.hpp>

namespace Xts {

enum class BlepType { Saw, Pulse, Triangle };
enum class UnitType { Sine, Additive, PolyBlep };
enum class UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };

enum class UnitModTarget 
{ 
  Amp, 
  Phase, 
  Pitch, 
  Panning,
  Frequency,
  BlepPulseWidth, 
  AdditiveRolloff 
};

struct XTS_ALIGN UnitModel
{
  XtsBool on;
  UnitType type;
  int32_t amp;
  int32_t panning;

  UnitNote note;
  int32_t octave;
  int32_t detune;
  int32_t pad__;

  BlepType blepType;
  int32_t blepPulseWidth;

  XtsBool additiveSub;
  int32_t additiveStep;
  int32_t additiveRolloff;
  int32_t additivePartials;

  TargetModsModel mods;
};
XTS_CHECK_SIZE(UnitModel, 88);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_UNIT_MODEL_HPP