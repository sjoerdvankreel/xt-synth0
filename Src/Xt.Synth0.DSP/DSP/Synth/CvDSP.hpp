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
  bool End(int env) const;
  CvState const& Output() const;
  EnvSample ReleaseAll(int env);
  EnvSample EnvOutput(int env) const;
public:
  CvDSP() = default;
  CvDSP(CvModel const* model, float velocity, float bpm, float rate);
};

inline CvState const& 
CvDSP::Output() const 
{ return _output; };

inline bool 
CvDSP::End(int env) const 
{ return _envs[env].End(); }

inline EnvSample 
CvDSP::EnvOutput(int env) const 
{ return _envs[env].Output(); };

} // namespace Xts
#endif // XTS_DSP_SYNTH_CV_DSP_HPP