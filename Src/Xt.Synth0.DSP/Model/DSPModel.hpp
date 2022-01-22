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
  int channel;
  float freq, rate;
  bool clip, bipolar;
  std::vector<int>* splits;
  std::vector<float>* samples;
public:
  PlotOutput() = default;
  PlotOutput(PlotOutput const&) = delete;
};

struct AudioState
{
  float envs[EnvCount];
public:
  AudioState() = default;
  AudioState(AudioState const&) = delete;
};

struct AudioInput
{
  int oct;
  UnitNote note;
  float bpm, rate;
public:
  AudioInput() = default;
  AudioInput(AudioInput const&) = delete;
  AudioInput(float r, float b, int o, UnitNote n):
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
class GeneratorDSP
{
protected:
  Model const* _model;
  AudioInput const* _input;
protected:
  GeneratorDSP() = default;
  GeneratorDSP(Model const* model, AudioInput const* input) :
  _model(model), _input(input) {}
};

template <class T, class Model> 
concept DSP = 
requires(T& dsp, Model const* model, AudioInput const* input, PlotOutput& out)
{
  { T(model, input) };
  { T::Plot(Model(), PlotInput(), out) } -> std::same_as<void>;
};

template <class T, class Model>
concept FiniteDSP = DSP<T, Model> &&
requires(T& dsp)
{
  { dsp.End() } -> std::same_as<bool>;
  { dsp.Release() } -> std::same_as<void>;
};

template <class T, class Model> 
concept StateSource = DSP<T, Model> &&
requires(T& dsp)
{ { dsp.Next() } -> std::same_as<float>; };

template <class T, class Model>
concept AudioSource = DSP<T, Model> &&
requires(T & dsp)
{ { dsp.Next(AudioState()) } -> std::same_as<AudioOutput>; };

} // namespace Xts
#endif // XTS_DSP_MODEL_HPP