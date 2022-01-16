#ifndef XTS_SYNTH_DSP_HPP
#define XTS_SYNTH_DSP_HPP

#include "EnvDSP.hpp"
#include "UnitDSP.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

struct SynthOutput
{
  bool end;
  float l, r;
  EnvOutput envs[TrackConstants::EnvCount];
  UnitOutput units[TrackConstants::UnitCount];
  SynthOutput() = default;
};

class SynthDSP
{
  friend class PlotDSP;
  EnvDSP _envs[TrackConstants::EnvCount];
  UnitDSP _units[TrackConstants::UnitCount];

public:
  void Release();
  void Init(int oct, UnitNote note);
  void Next(SynthModel const& synth, float rate, SynthOutput& output);

private:
  float GlobalAmp(SynthModel const& synth, SynthOutput const& output) const;
};

} // namespace Xts
#endif // XTS_SYNTH_DSP_HPP