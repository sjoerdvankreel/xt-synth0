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
  FilterDSP _filterDsp;
  struct CvModel const* _cv;
  struct AudioModel const* _audio;
  struct FilterModel const* _filter;
public:
  FilterPlot(CvModel const* cv, AudioModel const* audio, FilterModel const* filter, int index);
public:
  float Next();
  PeriodicParams Params() const;
  void Init(float bpm, float rate);
  float Frequency(float bpm, float rate) const;
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotState& state);
};

inline FilterPlot::
FilterPlot(CvModel const* cv, AudioModel const* audio, FilterModel const* filter, int index) : 
_index(index), 
_cv(cv), 
_audio(audio), 
_filter(filter) {}

inline float
FilterPlot::Frequency(float bpm, float rate) const 
{ return MidiNoteFrequency(5 * 12 + static_cast<int>(UnitNote::C)); }

} // namespace Xts
#endif // XTS_DSP_SYNTH_FILTER_PLOT_HPP