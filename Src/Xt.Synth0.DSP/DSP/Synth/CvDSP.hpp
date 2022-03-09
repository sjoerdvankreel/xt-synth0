#ifndef XTS_DSP_SYNTH_CV_DSP_HPP
#define XTS_DSP_SYNTH_CV_DSP_HPP

#include <DSP/Synth/LfoDSP.hpp>
#include <DSP/Synth/EnvDSP.hpp>
#include <DSP/Synth/CvState.hpp>
#include <Model/Synth/CvModel.hpp>

namespace Xts {

class CvDSP
{
  CvState _output;
  LfoDSP _lfos[XTS_SYNTH_LFO_COUNT];
  EnvDSP _envs[XTS_SYNTH_ENV_COUNT];
public:
  CvState const& Next();
  EnvSample ReleaseAll(int env);
  CvState const& Output() const { return _output; };
  EnvDSP const& Env(int env) const { return _envs[env]; }
public:
  CvDSP() = default;
  CvDSP(CvModel const* model, float velocity, float bpm, float rate);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_CV_DSP_HPP