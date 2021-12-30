using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public sealed class SynthModel : MainModel
	{
		public const int NativeSize = 1;

		[StructLayout(LayoutKind.Sequential)]
		public unsafe struct Native
		{
			internal AmpModel.Native amp;
			internal GlobalModel.Native global;
			internal fixed byte units[UnitCount * UnitModel.NativeSize];
		}

		public const int UnitCount = 3;

		static IEnumerable<UnitModel> MakeUnits()
		=> Enumerable.Range(0, UnitCount).Select(i => new UnitModel($"Unit {i + 1}"));

		public AmpModel Amp { get; } = new(nameof(Amp));
		public GlobalModel Global { get; } = new(nameof(Global));

		[JsonIgnore]
		public IReadOnlyList<UnitModel> Units => _units.Items;
		[JsonProperty(nameof(Units))]
		readonly ModelList<UnitModel> _units = new(MakeUnits());

		readonly List<AutoParam> _autoParams = new();
		public IReadOnlyList<AutoParam> AutoParams() => _autoParams;
		public AutoParam AutoParam(Param param)
		=> AutoParams().SingleOrDefault(a => a.Param == param);

		internal override IEnumerable<SubModel> ListSubModels()
		=> Units.Concat(new SubModel[] { Amp, Global });

		public SynthModel()
		{
			Units[0].On.Value = 1;
			int index = 1;
			foreach (var m in SubModels())
				_autoParams.AddRange(m.Params().Select(p => new AutoParam((GroupModel)m, p, index++)));
		}

		public unsafe void ToNative(Native* native)
		{
			Amp.ToNative(&native->amp);
			Global.ToNative(&native->global);
			for (int u = 0; u < UnitCount; u++)
				Units[u].ToNative(&((UnitModel.Native*)native->units)[u]);
		}

		public unsafe void FromNative(Native* native)
		{
			Amp.FromNative(&native->amp);
			Global.FromNative(&native->global);
			for (int u = 0; u < UnitCount; u++)
				Units[u].FromNative(&((UnitModel.Native*)native->units)[u]);
		}
	}
}