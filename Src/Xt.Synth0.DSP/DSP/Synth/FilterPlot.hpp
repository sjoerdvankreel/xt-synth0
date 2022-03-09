#ifndef XTS_DSP_SYNTH_FILTER_PLOT_HPP
#define XTS_DSP_SYNTH_FILTER_PLOT_HPP

#include <DSP/Plot.hpp>
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
  FilterDSP _filterDsp;
  struct CvModel const* _cv;
  struct AudioModel const* _audio;
  struct FilterModel const* _filter;
public:
  float Next();
  PeriodicParams Params() const;
  void Init(float bpm, float rate);
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotOutput& output);
public:
  float Frequency(float bpm, float rate) const { return MidiNoteFrequency(5 * 12 + static_cast<int>(UnitNote::C)); }
  FilterPlot(CvModel const* cv, AudioModel const* audio, FilterModel const* filter, int index) : _index(index), _cv(cv), _audio(audio), _filter(filter) {}
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_FILTER_PLOT_HPP