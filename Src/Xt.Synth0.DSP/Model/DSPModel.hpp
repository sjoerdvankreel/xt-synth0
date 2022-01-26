#ifndef XTS_DSP_MODEL_HPP
#define XTS_DSP_MODEL_HPP

#include "SynthModel.hpp"
#include <vector>

namespace Xts {

struct PlotInput
{
  int32_t hold;
  float bpm, pixels;
public:
  PlotInput() = default;
  PlotInput(PlotInput const&) = delete;
};

struct PlotOutput
{
  int channel;
  float freq, rate;
  bool clip, bipolar;
  std::vector<int>* splits;
  std::vector<float>* samples;
public:
  PlotOutput() = default;
  PlotOutput(PlotOutput const&) = delete;
};

struct SynthState
{
  float lfos[LfoCount];
  float envs[EnvCount];
public:
  SynthState() = default;
  SynthState(SynthState const&) = delete;
};

struct SynthInput
{
  int oct;
  UnitNote note;
  float bpm, rate;
public:
  SynthInput() = default;
  SynthInput(SynthInput const&) = delete;
  SynthInput(float r, float b, int o, UnitNote n):
  oct(o), note(n), bpm(b), rate(r) {}
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
  float Mono() const { return l + r; } 
  AudioOutput operator*(float f) const
  { return AudioOutput(l * f, r * f); }
  AudioOutput& operator+=(AudioOutput const& rhs) 
  { l += rhs.l; r += rhs.r; return *this; }
};

template <class Model>
class DSPBase
{
protected:
  Model const* _model;
  SynthInput const* _input;
protected:
  DSPBase() = default;
  DSPBase(Model const* model, SynthInput const* input) :
  _model(model), _input(input) {}
};

template <class T, class Model> 
concept DSP = std::is_base_of<DSPBase<Model>, T>::value &&
requires(T& dsp, Model const* model, SynthInput const* input, PlotOutput& out)
{ { T(model, input) }; };

template <class T, class Model> 
concept StateSourceDSP = PlottableDSP<T, Model> &&
requires(T& dsp)
{ { dsp.Next() } -> std::same_as<float>; };

template <class T, class Model>
concept AudioSourceDSP = PlottableDSP<T, Model> &&
requires(T & dsp)
{ { dsp.Next(SynthState()) } -> std::same_as<AudioOutput>; };

template <class T, class Model> 
concept PlottableDSP = DSP<T, Model> &&
requires(T& dsp, Model const* model, SynthInput const* input, PlotOutput& out)
{ { T::Plot(Model(), PlotInput(), out) } -> std::same_as<void>; };

template <class T, class Model>
concept FiniteDSP = PlottableDSP<T, Model> &&
requires(T& dsp)
{
  { dsp.End() } -> std::same_as<bool>;
  { dsp.Release() } -> std::same_as<void>;
};

} // namespace Xts
#endif // XTS_DSP_MODEL_HPP