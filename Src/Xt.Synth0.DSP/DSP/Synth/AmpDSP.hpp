#ifndef XTS_DSP_SYNTH_AMP_DSP_HPP
#define XTS_DSP_SYNTH_AMP_DSP_HPP

#include <DSP/AudioSample.hpp>
#include <DSP/Synth/ModDSP.hpp>
#include <Model/Synth/Config.hpp>
#include <Model/Synth/AmpModel.hpp>

namespace Xts {

class AmpDSP
{
  float _amp;
  float _level;
  float _panning;
  ModDSP _ampMod;
  ModDSP _panMod;
  FloatSample _output;
  AmpModel const* _model;
  float _unitAmount[XTS_SYNTH_UNIT_COUNT];
  float _filterAmount[XTS_SYNTH_FILTER_COUNT];
public:
  AmpDSP() = default;
  AmpDSP(AmpModel const* model, float velocity);
public:
  int Env() const;
  FloatSample Output() const;
  FloatSample Next(struct CvState const& cv, struct AudioState const& audio);
  static void Plot(struct SynthModel const& model, struct PlotInput const& input, struct PlotOutput& output);
};

inline FloatSample
AmpDSP::Output() const
{ return _output; }

inline int
AmpDSP::Env() const
{ return static_cast<int>(_model->ampEnvSource); }

} // namespace Xts
#endif // XTS_DSP_SYNTH_AMP_DSP_HPP