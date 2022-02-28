#ifndef XTS_CV_DSP_HPP
#define XTS_CV_DSP_HPP

#include <DSP/Synth/CvState.hpp>
#include "LfoDSP.hpp"
#include "EnvDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class CvDSP
{
  CvState _output;
  LfoDSP _lfos[LfoCount];
  EnvDSP _envs[EnvelopeCount];
public:
  CvDSP() = default;
  CvDSP(CvModel const* model, float velo, float bpm, float rate);
public:
  CvState const& Next();
  EnvelopeSample ReleaseAll(int env);
  CvState const& Output() const { return _output; };
  bool End(int env) const { return _envs[env].End(); }
  EnvelopeSample EnvOutput(int env) const { return _envs[env].Output(); };
};

} // namespace Xts
#endif // XTS_CV_DSP_HPP