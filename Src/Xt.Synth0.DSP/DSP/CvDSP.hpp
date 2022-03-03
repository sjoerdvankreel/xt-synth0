#ifndef XTS_CV_DSP_HPP
#define XTS_CV_DSP_HPP

#include <Model/Synth/Config.hpp>
#include <DSP/Synth/CvState.hpp>
#include <DSP/Synth/LfoDSP.hpp>
#include "EnvDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class CvDSP
{
  CvState _output;
  LfoDSP _lfos[XTS_SYNTH_LFO_COUNT];
  EnvDSP _envs[XTS_SYNTH_ENV_COUNT];
public:
  CvDSP() = default;
  CvDSP(CvModel const* model, float velo, float bpm, float rate);
public:
  CvState const& Next();
  EnvSample ReleaseAll(int env);
  CvState const& Output() const { return _output; };
  bool End(int env) const { return _envs[env].End(); }
  EnvSample EnvOutput(int env) const { return _envs[env].Output(); };
};

} // namespace Xts
#endif // XTS_CV_DSP_HPP