using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public sealed class GlobalModel : IGroupModel<GlobalModel>
	{
		internal const int Size = 1;

		[StructLayout(LayoutKind.Sequential)]
		internal struct Native
		{
			internal int bpm;
			internal int hmns;
			internal int plot;
			internal int method;
		}

		static readonly string[] Methods = Enum.
			GetValues<SynthMethod>().Select(v => v.ToString()).ToArray();

		static readonly ParamInfo BpmInfo = new DiscreteInfo(
			nameof(Bpm), "Tempo", 1, 255, 120);
		static readonly ParamInfo PlotInfo = new DiscreteInfo(
			nameof(Plot), "Plot unit", 1, SynthModel.UnitCount, 1);
		static readonly ParamInfo HmnsInfo = new ExpInfo(
			nameof(Hmns), "Additive harmonics", 0, 10, 4);
		static readonly ParamInfo MethodInfo = new EnumInfo<SynthMethod>(
			nameof(Method), "Method (PolyBLEP, Additive, Naive)", Methods);

		public Param Bpm { get; } = new(BpmInfo);
		public Param Hmns { get; } = new(HmnsInfo);
		public Param Plot { get; } = new(PlotInfo);
		public Param Method { get; } = new(MethodInfo);

		public int NativeSize() => Size;
		public string Name() => "Global";

		public Param[][] ParamGroups() => new[]
		{
			new[] { Bpm, Plot },
			new[] { Method, Hmns }
		};

		public void CopyTo(GlobalModel model)
		{
			model.Bpm.Value = Bpm.Value;
			model.Hmns.Value = Hmns.Value;
			model.Plot.Value = Plot.Value;
			model.Method.Value = Method.Value;
		}

		public unsafe void ToNative(IntPtr native)
		{
			Native* p = (Native*)native;
			p->bpm = Bpm.Value;
			p->hmns = Hmns.Value;
			p->plot = Plot.Value;
			p->method = Method.Value;
		}

		public unsafe void FromNative(IntPtr native)
		{
			Native* p = (Native*)native;
			Bpm.Value = p->bpm;
			Hmns.Value = p->hmns;
			Plot.Value = p->plot;
			Method.Value = p->method;
		}

		public void RegisterParams(Action<Param> register)
		{
			register(Bpm);
			register(Hmns);
			register(Plot);
			register(Method);
		}
	}
}