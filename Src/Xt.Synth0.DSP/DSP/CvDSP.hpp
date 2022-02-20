#ifndef XTS_CV_DSP_HPP
#define XTS_CV_DSP_HPP

#include "LfoDSP.hpp"
#include "EnvDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class CvDSP
{
  CvState _output;
  LfoDSP _lfos[LfoCount];
  EnvDSP _envs[EnvCount];
public:
  CvDSP() = default;
  CvDSP(CvModel const* model, float velo, float bpm, float rate);
public:
  void Release();
  CvState const& Next();
  CvState const& Output() const { return _output; };
  bool End(int env) const { return _envs[env].End(); }
};

} // namespace Xts
#endif // XTS_CV_DSP_HPP