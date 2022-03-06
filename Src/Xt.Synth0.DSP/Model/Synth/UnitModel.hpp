#ifndef XTS_MODEL_SYNTH_UNIT_MODEL_HPP
#define XTS_MODEL_SYNTH_UNIT_MODEL_HPP

#include <Model/Model.hpp>
#include <Model/Synth/ModModel.hpp>

namespace Xts {

enum class BlepType { Saw, Pulse, Triangle };
enum class UnitType { Sine, Additive, PolyBlep };
enum class UnitModTarget { Amp, Pan, Pw, Roll, Freq, Pitch, Phase };
enum class UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B };

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

  ModModel<UnitModTarget> mod1;
  ModModel<UnitModTarget> mod2;
};
XTS_CHECK_SIZE(UnitModel, 88);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_UNIT_MODEL_HPP