#ifndef XTS_DSP_SYNTH_VOICE_FILTER_DSP_HPP
#define XTS_DSP_SYNTH_VOICE_FILTER_DSP_HPP

#include <DSP/Synth/FilterDSP.hpp>
#include <DSP/Synth/SourceTargetModsDSP.hpp>
#include <Model/Synth/SynthConfig.hpp>

namespace Xts {

class VoiceFilterDSP
{
  int _index;
  FilterDSP _dsp;
  FloatSample _output;
  SourceTargetModsDSP _mods;
  struct VoiceFilterModel const* _model;
private:
  FloatSample GenerateComb();
  FloatSample GenerateStateVar();
public:
  FloatSample Output() const { return _output; };
  FloatSample Next(struct CvState const& cv, struct AudioState const& audio);
public:
  VoiceFilterDSP() = default;
  VoiceFilterDSP(struct VoiceFilterModel const* model, int index, float rate);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_VOICE_FILTER_DSP_HPP