#ifndef XTS_GLOBAL_DSP_HPP
#define XTS_GLOBAL_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class GlobalDSP: 
private DSPBase<GlobalModel>
{
public:
  GlobalDSP() = default;
  GlobalDSP(GlobalModel const* model, SynthInput const* input):
  DSPBase(model, input) {}
public:
  float Amp(SynthState const& state);
  AmpSource Src() const { return _model->src; }
};
static_assert(DSP<GlobalDSP, GlobalModel>);

} // namespace Xts
#endif // XTS_GLOBAL_DSP_HPP