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
  friend class UnitDSP;
  UnitModel() = default;
  UnitModel(UnitModel const&) = delete;
private:
  XtsBool on;
  UnitType type;
  UnitNote note;
  XtsBool addSub;
  BlepType blepType;
  int32_t amt1, amt2;
  ModSource src1, src2;
  UnitModTarget tgt1, tgt2;
  int32_t amp, pan, oct, dtn, pw;
  int32_t addParts, addStep, addRoll, pad__;
};
XTS_CHECK_SIZE(UnitModel, 80);


} // namespace Xts
#endif // XTS_MODEL_SYNTH_UNIT_MODEL_HPP