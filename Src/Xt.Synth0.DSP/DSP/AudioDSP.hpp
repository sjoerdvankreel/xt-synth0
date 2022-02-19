#ifndef XTS_AUDIO_DSP_HPP
#define XTS_AUDIO_DSP_HPP

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
    void Next();
    void Release();
    CvState const& Output() const { return _output; };
  public:
    CvDSP() = default;
    CvDSP(CvModel const* model, float velo, float bpm, float rate);
  };

} // namespace Xts
#endif // XTS_AUDIO_DSP_HPP