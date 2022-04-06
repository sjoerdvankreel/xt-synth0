#ifndef XTS_DSP_SYNTH_VOICE_FILTER_DSP_HPP
#define XTS_DSP_SYNTH_VOICE_FILTER_DSP_HPP

#include <DSP/Synth/FilterDSP.hpp>
#include <DSP/Synth/TargetModsDSP.hpp>
#include <Model/Shared/NoteType.hpp>
#include <Model/Synth/SynthConfig.hpp>

namespace Xts {

class VoiceFilterDSP
{
  int _index;
  FilterDSP _dsp;
  FloatSample _output;
  TargetModsDSP _mods;
  float _keyboardBase;
  struct VoiceFilterModel const* _model;
private:
  FloatSample Generate();
  FloatSample GenerateComb();
public:
  FloatSample Output() const { return _output; };
  FloatSample Next(struct CvState const& cv, struct AudioState const& audio);
public:
  VoiceFilterDSP() = default;
  VoiceFilterDSP(struct VoiceFilterModel const* model, int octave, NoteType note, int index, float rate);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_VOICE_FILTER_DSP_HPP