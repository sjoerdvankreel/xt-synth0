#ifndef XTS_DSP_SHARED_MODULATE_HPP
#define XTS_DSP_SHARED_MODULATE_HPP

#include <DSP/Shared/CvSample.hpp>

namespace Xts {

float
Modulate(CvSample carrier, CvSample modulator, float amount);

} // namespace Xts
#endif // XTS_DSP_SHARED_MODULATE_HPP