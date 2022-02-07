#ifndef XTS_DSP_MODEL_HPP
#define XTS_DSP_MODEL_HPP

#include "SynthModel.hpp"
#include <vector>
#include <string>
#include <complex>

namespace Xts {

class SourceDSP;

struct HSplit
{
  int pos;
  std::wstring marker;
  HSplit(int p, std::wstring const& m):
  pos(p), marker(m) {}
};

struct VSplit
{
  float pos;
  std::wstring marker;
  VSplit(float p, std::wstring const& m):
    pos(p), marker(m) {}
};

struct PlotInput
{
  int32_t hold;
  XtsBool spec;
  float bpm, rate, pixels;
public:
  PlotInput() = default;
  PlotInput(PlotInput const&) = delete;
};

struct PlotOutput
{
  bool clip;
  int channel;
  float freq, rate, min, max;
  std::vector<float>* samples;
  std::vector<HSplit>* hSplits;
  std::vector<VSplit>* vSplits;
  std::vector<std::complex<float>>* fftData;
  std::vector<std::complex<float>>* fftScratch;
public:
  PlotOutput() = default;
  PlotOutput(PlotOutput const&) = delete;
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
  AudioOutput operator/(float f) const
  { return AudioOutput(l / f, r / f); }
  AudioOutput operator+(float f) const
  { return AudioOutput(l + f, r + f); }
  AudioOutput operator-(float f) const
  { return AudioOutput(l - f, r - f); }
  AudioOutput& operator+=(AudioOutput const& rhs) 
  { l += rhs.l; r += rhs.r; return *this; }
};

template <class Model, class Input, class Output>
class DSPBase
{
protected:
  Output _value;
  Model const* _model;
  Input const* _input;
protected:
  DSPBase() = default;
  DSPBase(Model const* model, Input const* input) :
  _model(model), _input(input), _value() {}
};

template <class T, class Model, class Input, class Output>
concept DSP = std::derived_from<T, DSPBase<Model, Input, Output>> &&
requires(T const& dsp, Model const* model, Input const* input)
{
  { T(model, input) }; 
  { dsp.Value() } -> std::same_as<Output>;
};

template <class T, class Model, class Input, class Output>
concept ReleaseableDSP = DSP<T, Model, Input, Output> &&
requires(T& dsp)
{ { dsp.Release() } -> std::same_as<void>; };

template <class T, class Model, class Input, class Output>
concept FiniteSourceDSP = DSP<T, Model, Input, Output> &&
requires(T const& dsp)
{ { dsp.End() } -> std::same_as<bool>; };

template <class T, class Model, class Input, class Output>
concept FiniteDependentDSP = DSP<T, Model, Input, Output> &&
requires(T const& dsp, SourceDSP const& source)
{ { dsp.End(source) } -> std::same_as<bool>; };

template <class T, class Model, class Input, class Output>
concept PlottableSourceDSP = DSP<T, Model, Input, Output> &&
requires(PlotOutput& out)
{ { T::Plot(Model(), PlotInput(), out) } -> std::same_as<void>; };

template <class T, class Model, class Input, class Output>
concept PlottableDependentDSP = DSP<T, Model, Input, Output> &&
requires(SourceModel const& source, PlotOutput& out)
{ { T::Plot(Model(), source, PlotInput(), out) } -> std::same_as<void>; };

template <class T, class Model> 
concept StateSourceDSP = PlottableSourceDSP<T, Model, SourceInput, float> &&
requires(T& dsp)
{ { dsp.Next() } -> std::same_as<void>; };

template <class T, class Model> 
concept StatePipeDSP = PlottableDependentDSP<T, Model, SourceInput, float> &&
requires(T& dsp, SourceDSP const& source)
{ { dsp.Next(source) } -> std::same_as<void>; };

template <class T, class Model>
concept AudioSourceDSP = PlottableDependentDSP<T, Model, AudioInput, AudioOutput> &&
requires(T & dsp, SourceDSP const& source)
{ { dsp.Next(source) } -> std::same_as<void>; };

} // namespace Xts
#endif // XTS_DSP_MODEL_HPP