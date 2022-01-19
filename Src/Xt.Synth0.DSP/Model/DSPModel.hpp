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
  { Output(dsp.Next(Model(), AudioInput())) };
  { dsp.Init(Model(), AudioInput()) } -> std::convertible_to<void>;
  { dsp.Release(Model(), AudioInput()) } -> std::convertible_to<void>;
  { dsp.Plot(Model(), PlotInput(), out) } -> std::convertible_to<void>;
};

} // namespace Xts
#endif // XTS_DSP_MODEL_HPP