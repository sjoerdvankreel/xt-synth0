#ifndef XTS_DSP_HPP
#define XTS_DSP_HPP

#include <DSP/Synth/AudioState.hpp>
#include <DSP/Synth/CvState.hpp>
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

#include <cmath>
#include <cstdint>
#include <cassert>
#include <complex>

namespace Xts {

float Modulate(float val, bool bip, float amt, CvSample cv);
CvSample ModulationInput(CvState const& cv, ModSource src);
ModInput ModulationInput(CvState const& cv, ModSource src1, ModSource src2);

} // namespace Xts
#endif // XTS_DSP_HPP