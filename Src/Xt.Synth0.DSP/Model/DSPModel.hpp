#ifndef XTS_DSP_MODEL_HPP
#define XTS_DSP_MODEL_HPP

#include "SynthModel.hpp"
#include <vector>

namespace Xts {

struct PlotInput
{
  float bpm, pixels;
public:
  PlotInput() = default;
  PlotInput(PlotInput const&) = delete;
};

struct PlotOutput
{
  float freq, rate;
  bool clip, bipolar;
  std::vector<int> splits;
  std::vector<float> samples;
public:
  PlotOutput() = default;
  PlotOutput(PlotOutput const&) = delete;
};

struct AudioOutput
{
  float l;
  float r;
public:
  AudioOutput(): l(0.0f), r(0.0f) {}
  AudioOutput(AudioOutput const&) = default;
  AudioOutput(float l, float r) : l(l), r(r) {}
public:
  AudioOutput operator*(float f)
  { return AudioOutput(l * f, r * f); }
  AudioOutput& operator+=(AudioOutput const& rhs) 
  { l += rhs.l; r += rhs.r; return *this; }
};

struct AudioInput
{
  int oct;
  UnitNote note;
  float bpm, rate;
public:
  AudioInput() = default;
  AudioInput(AudioInput const&) = delete;
  AudioInput(float r, float b, int o, UnitNote n) :
    oct(o), note(n), bpm(b), rate(r) {}
};

template <class DSP, class Model, class Output>
concept Generator = requires(DSP& dsp, PlotOutput& out)
{
  { dsp.End() } -> std::same_as<bool>;
  { dsp.Init(Model(), AudioInput()) } -> std::same_as<void>;
  { dsp.Next(Model(), AudioInput()) } -> std::same_as<Output>;
  { dsp.Release(Model(), AudioInput()) } -> std::same_as<void>;
  { dsp.Plot(Model(), PlotInput(), out) } -> std::same_as<void>;
};

} // namespace Xts
#endif // XTS_DSP_MODEL_HPP