#ifndef XTS_CV_DSP_HPP
#define XTS_CV_DSP_HPP

#include "LfoDSP.hpp"
#include "EnvDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class CVDSP
{
  CVState _output;
  LfoDSP _lfos[LfoCount];
  EnvDSP _envs[EnvCount];
public:
  void Next();
  void Release();
  CVState const& Output() const { return _output; };
public:
  CVDSP() = default;
  CVDSP(CVModel const* model, float velo, float bpm, float rate);
};

} // namespace Xts
#endif // XTS_CV_DSP_HPP