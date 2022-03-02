#ifndef XTS_MODEL_SYNTH_AMP_MODEL_HPP
#define XTS_MODEL_SYNTH_AMP_MODEL_HPP

#include <Model/Model.hpp>
#include <Model/Synth/ModModel.hpp>

namespace Xts {

enum class AmpLfo { LFO1, LFO2, LFO3 };
enum class AmpEnv { Env1, Env2, Env3 };
struct XTS_ALIGN AmpModel
{
  friend class AmpDSP;
  friend class PlotDSP;
  AmpModel() = default;
  AmpModel(AmpModel const&) = delete;
private:
  AmpEnv envSrc;
  AmpLfo lvlSrc;
  ModSource panSrc;
  int32_t units[XTS_SYNTH_UNIT_COUNT];
  int32_t flts[XTS_SYNTH_FILTER_COUNT];
  int32_t lvl, pan, lvlAmt, panAmt, pad__;
};
XTS_CHECK_SIZE(AmpModel, 56);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_AMP_MODEL_HPP