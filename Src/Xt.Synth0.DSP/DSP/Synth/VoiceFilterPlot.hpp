#ifndef XTS_DSP_SYNTH_VOICE_FILTER_PLOT_HPP
#define XTS_DSP_SYNTH_VOICE_FILTER_PLOT_HPP

#include <DSP/Shared/Plot.hpp>
#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Synth/AudioDSP.hpp>
#include <DSP/Synth/VoiceFilterDSP.hpp>
#include <Model/Synth/SynthModel.hpp>

namespace Xts {

class VoiceFilterPlot :
public PeriodicPlot
{
  int _index;
  CvDSP _cvDsp;
  AudioDSP _audioDsp;
  LfoDSP _globalLfoDsp;
  VoiceFilterDSP _filterDsp;
  struct SynthModel const* _model;
public:
  float Next();
  PeriodicParams Params() const;
  void Init(float bpm, float rate);
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotState& state);
public:
  VoiceFilterPlot(struct SynthModel const* model, int index) : _model(model), _index(index) {};
  float Frequency(float bpm, float rate) const { return UnitDSP::Frequency(_model->voice.audio.units[0], 4, NoteType::C); }
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_VOICE_FILTER_PLOT_HPP