using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class SynthModel : MainModel
	{
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
	}
}