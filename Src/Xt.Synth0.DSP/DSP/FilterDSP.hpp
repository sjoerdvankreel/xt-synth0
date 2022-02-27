#ifndef XTS_FILTER_DSP_HPP
#define XTS_FILTER_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class FilterDSP
{
  AudioOutput _output;
  int _index;
  float _amt1, _amt2;
  float _units[UnitCount];
  float _flts[FilterCount];
  FilterModel const* _model;
  int _cbdPlus, _cbdMin;
  float _cbgPlus, _cbgMin;
  float _bqa[3], _bqb[3];
  AudioOutput _bqx[3], _bqy[3];
  AudioOutput _cbx[256], _cby[256];
private:
  void InitComb();
  void InitBQ(float rate);
  AudioOutput GenerateBQ(AudioOutput audio);
  AudioOutput GenerateComb(AudioOutput audio);
public:
  FilterDSP() = default;
  FilterDSP(FilterModel const* model, int index, float rate);
public:
  AudioOutput Output() const { return _output; };
  AudioOutput Next(CvState const& cv, AudioState const& audio);
  static void Plot(FilterModel const& model, CvModel const& cvModel, AudioModel const& AudioModel, bool spec, int index, PlotInput const& input, PlotOutput& output);
};

} // namespace Xts
#endif // XTS_FILTER_DSP_HPP