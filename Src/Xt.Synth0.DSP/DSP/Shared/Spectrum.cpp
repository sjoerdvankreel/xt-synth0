#include <DSP/Shared/Plot.hpp>
#include <DSP/Shared/Utility.hpp>
#include <DSP/Shared/Spectrum.hpp>

#include <vector>
#include <cassert>
#include <complex>

// https://stackoverflow.com/questions/604453/analyze-audio-using-fast-fourier-transform
// https://dsp.stackexchange.com/questions/46692/calculating-1-3-octave-spectrum-from-fft-dft

namespace Xts {

typedef std::vector<std::complex<float>> ComplexVector;

static void
MonoMarkers(std::vector<VerticalMarker>& markers)
{
  markers.clear();
  markers.emplace_back(1.0f - (1.0f / 1.0f), L"1.0");
  markers.emplace_back(1.0f - (1.0f / 2.0f), L".50");
  markers.emplace_back(1.0f - (1.0f / 4.0f), L".25");
  markers.emplace_back(1.0f - (1.0f / 8.0f), L"");
  markers.emplace_back(1.0f - (1.0f / 16.0f), L"");
  markers.emplace_back(1.0f - (1.0f / 32.0f), L"");
  markers.emplace_back(1.0f, L"0.0");
}

static void
StereoMarkers(std::vector<VerticalMarker>& markers)
{
  markers.clear();
  markers.emplace_back(1.0f - (1.0f / 1.0f), L"1.0");
  markers.emplace_back(1.0f - (3.0f / 4.0f), L".50");
  markers.emplace_back(1.0f - (5.0f / 8.0f), L".25");
  markers.emplace_back(1.0f - (9.0f / 16.0f), L"");
  markers.emplace_back(1.0f - (1.0f / 2.0f), L"0/1");
  markers.emplace_back(1.0f - (1.0f / 4.0f), L".50");
  markers.emplace_back(1.0f - (1.0f / 8.0f), L".25");
  markers.emplace_back(1.0f - (1.0f / 16.0f), L"");
  markers.emplace_back(1.0f, L"0.0");
}

static void
HorizontalMarkers(std::vector<HorizontalMarker>& markers)
{
  markers.clear();
  for (int oct = 0; oct < 12; oct++)
    markers.emplace_back(HorizontalMarker(oct * 12, oct >= 2 ? std::to_wstring(oct - 2) : L""));
  markers.emplace_back(HorizontalMarker(143, L""));
}

static void
Fft(std::complex<float>* x, std::complex<float>* scratch, size_t count)
{
  if (count < 2) return;
  assert(count == NextPowerOf2(count));
  std::complex<float>* even = scratch;
  std::complex<float>* odd = scratch + count / 2;
  for (size_t i = 0; i < count / 2; i++) even[i] = x[i * 2];
  for (size_t i = 0; i < count / 2; i++) odd[i] = x[i * 2 + 1];
  Fft(odd, x, count / 2);
  Fft(even, x, count / 2);
  for (size_t i = 0; i < count / 2; i++)
  {
    float im = -2.0f * PIF * i / count;
    std::complex<float> t = std::polar(1.0f, im) * odd[i];
    x[i] = even[i] + t;
    x[i + count / 2] = even[i] - t;
  }
}

static float
Power(ComplexVector const& fft, float rate, int octave, int note)
{
  float result = 0.0f;
  float freq2Bin = rate / (fft.size() * 2.0f);
  float midi = static_cast<float>(octave * 12 + note);
  size_t bin1 = static_cast<size_t>(MidiNoteFrequency(midi) * freq2Bin);
  size_t bin2 = static_cast<size_t>(MidiNoteFrequency(midi + 1) * freq2Bin);
  for (size_t i = bin1; i < bin2 && i < fft.size(); i++)
    result += fft[i].real() * fft[i].real() + fft[i].imag() * fft[i].imag();
  return Sanity(std::sqrtf(result));
}

static void
Fft(std::vector<float> const& x, ComplexVector& fft, ComplexVector& scratch)
{
  fft.resize(x.size());
  scratch.resize(x.size());
  assert(x.size() == NextPowerOf2(x.size()));
  for (size_t i = 0; i < x.size(); i++) fft[i] = std::complex<float>(x[i], 0.0f);
  Fft(fft.data(), scratch.data(), x.size());
}

static void 
Spectrum(std::vector<float>& x, ComplexVector& fft, ComplexVector& scratch, float rate)
{
  float max = 0;
  assert(x.size() > 0 && x.size() == NextPowerOf2(x.size()));
  Fft(x, fft, scratch);
  x.clear();
  fft.erase(fft.begin() + fft.size() / 2, fft.end());
  for(int oct = 0; oct < 12; oct++)
    for(int note = 0; note < 12; note++)
      x.push_back(Power(fft, rate, oct, note));
  for (size_t i = 0; i < x.size(); i++) max = std::max(x[i], max);
  for (size_t i = 0; i < x.size(); i++) x[i] = max == 0.0f? 0.0f: x[i] / max;
}

void
TransformToSpectrum(PlotOutput& output)
{
  output.min = 0.0f;
  output.max = 1.0f;
  if (output.left->empty()) return;
  output.left->resize(NextPowerOf2(output.left->size()));
  assert(output.left->size() >= static_cast<size_t>(output.rate));
  Spectrum(*output.left, *output.fft, *output.scratch, output.rate);
  MonoMarkers(*output.vertical);
  HorizontalMarkers(*output.horizontal);
  if (!output.stereo) return;
  StereoMarkers(*output.vertical);
  output.right->resize(NextPowerOf2(output.right->size()));
  Spectrum(*output.right, *output.fft, *output.scratch, output.rate);
}

} // namespace Xts