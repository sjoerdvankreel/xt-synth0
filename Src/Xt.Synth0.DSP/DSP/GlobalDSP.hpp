#ifndef XTS_GLOBAL_DSP_HPP
#define XTS_GLOBAL_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class GlobalDSP
{
private:
  GlobalModel const* _model;
public:
  float Amp(SynthState const& state);
public:
  GlobalDSP() = default;
  GlobalDSP(GlobalModel const* model): _model(model) {}
};

} // namespace Xts
#endif // XTS_GLOBAL_DSP_HPP