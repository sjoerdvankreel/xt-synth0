#ifndef XTS_AMP_DSP_HPP
#define XTS_AMP_DSP_HPP

#include "DSP.hpp"
#include "CvDSP.hpp"
#include "AudioDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class AmpDSP
{
  float _amp;
  AudioOutput _output;
  AmpModel const* _model;
  float _units[UnitCount];
  float _flts[FilterCount];
  float _pan, _lvlAmt, _panAmt, _lvl;
public:
  AmpDSP() = default;
  AmpDSP(AmpModel const* model, float velo);
public:
  AudioOutput Output() const { return _output; };
  int Env() const { return static_cast<int>(_model->envSrc); }
  AudioOutput Next(CvState const& cv, AudioState const& audio);
  static void Plot(AmpModel const& model, EnvModel const& envModel, CvModel const& cvModel, AudioModel const& audio, PlotInput const& input, PlotOutput& output);
};

} // namespace Xts
#endif // XTS_AMP_DSP_HPP