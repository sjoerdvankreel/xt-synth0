#ifndef XTS_DSP_SYNTH_FILTER_PLOT_HPP
#define XTS_DSP_SYNTH_FILTER_PLOT_HPP

#include <DSP/Shared/Plot.hpp>
#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Synth/AudioDSP.hpp>
#include <DSP/Synth/FilterDSP.hpp>

namespace Xts {

class FilterPlot :
public PeriodicPlot
{
  int _index;
  CvDSP _cvDsp;
  AudioDSP _audioDsp;
  LfoDSP _globalLfoDsp;
  FilterDSP _filterDsp;
  struct SynthModel const* _model;
public:
  float Next();
  PeriodicParams Params() const;
  void Init(int lpb, float bpm, float rate);
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotState& state);
public:
  FilterPlot(struct SynthModel const* model, int index) : _model(model), _index(index) {};
  float Frequency(int lpb, float bpm, float rate) const { return MidiNoteFrequency(5 * 12 + static_cast<int>(UnitNote::C)); }
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_FILTER_PLOT_HPP