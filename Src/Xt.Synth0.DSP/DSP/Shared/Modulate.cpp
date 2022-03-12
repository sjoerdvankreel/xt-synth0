#include <DSP/Shared/Modulate.hpp>

namespace Xts {

float
Modulate(CvSample carrier, CvSample modulator, float amount)
{
  double range = 0.0f;
  if (amount == 0.0f) return carrier.value;
  double carrierValue = EpsilonToZero(carrier.value);
  double modulatorValue = EpsilonToZero(modulator.value);
  if (!modulator.bipolar && amount > 0.0f) range = 1.0 - carrierValue;
  if (!modulator.bipolar && !carrier.bipolar && amount < 0.0f) range = carrierValue;
  if (!modulator.bipolar && carrier.bipolar && amount < 0.0f) range = 1.0 + carrierValue;
  if (modulator.bipolar && carrier.bipolar) range = 1.0 - std::fabs(carrierValue);
  if (modulator.bipolar && !carrier.bipolar) range = 0.5 - std::fabs(carrierValue - 0.5f);
  double result = carrierValue + modulatorValue * amount * range;
  if (carrier.bipolar) BipolarSanity(result);
  else UnipolarSanity(result);
  return static_cast<float>(result);
}

} // namespace Xts