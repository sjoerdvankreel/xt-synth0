#ifndef XTS_DSP_MODEL_HPP
#define XTS_DSP_MODEL_HPP

#include "SynthModel.hpp"
#include <vector>
#include <complex>

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
  std::vector<std::complex<float>>* fft;
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

struct KeyInput
{
  int oct;
  UnitNote note;
public:
  KeyInput() = default;
  KeyInput(KeyInput const&) = default;
  KeyInput(int oct, UnitNote note):
  oct(oct), note(note) {}
};

struct SourceInput
{
  float bpm, rate;
public:
  SourceInput() = default;
  SourceInput(SourceInput const&) = default;
  SourceInput(float rate, float bpm): 
  rate(rate), bpm(bpm) {}
};

struct AudioInput
{
  KeyInput key;
  SourceInput source;
public:
  AudioInput() = default;
  AudioInput(AudioInput const&) = delete;
  AudioInput(SourceInput const& source, KeyInput const& key):
  source(source), key(key) {}
};

template <class Model, class Input, class Output>
class DSPBase
{
protected:
  Output _value;
  Model const* _model;
  Input const* _input;
public:
  Output const& Value() const { return _value; }
protected:
  DSPBase() = default;
  DSPBase(Model const* model, Input const* input) :
  _model(model), _input(input), _value() {}
};

template <class T, class Model, class Input, class Output>
concept DSP = std::derived_from<T, DSPBase<Model, Input, Output>> &&
requires(T& dsp, Model const* model, Input const* input, PlotOutput& out)
{ 
  { T(model, input) }; 
  { T::Plot(Model(), PlotInput(), out) } -> std::same_as<void>;
};

template <class T, class Model> 
concept StateSourceDSP = DSP<T, Model, SourceInput, float> &&
requires(T& dsp)
{ { dsp.Next() } -> std::same_as<void>; };

template <class T, class Model> 
concept StatePipeDSP = DSP<T, Model, SourceInput, float> &&
requires(T& dsp)
{ { dsp.Next(SynthState()) } -> std::same_as<void>; };

template <class T, class Model>
concept AudioSourceDSP = DSP<T, Model, AudioInput, AudioOutput> &&
requires(T & dsp)
{ { dsp.Next(SynthState()) } -> std::same_as<void>; };

template <class T, class Model, class Input, class Output>
concept FiniteDSP = DSP<T, Model, Input, Output> &&
requires(T& dsp)
{
  { dsp.End() } -> std::same_as<bool>;
  { dsp.Release() } -> std::same_as<void>;
};

} // namespace Xts
#endif // XTS_DSP_MODEL_HPP