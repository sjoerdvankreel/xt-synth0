using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class PatternRow : SubModel
	{
		public const int MaxFxCount = 2;
		public const int MaxKeyCount = 3;

		static IEnumerable<PatternFx> MakeFx()
		=> Enumerable.Repeat(0, MaxFxCount).Select(_ => new PatternFx());
		static IEnumerable<PatternKey> MakeKeys()
		=> Enumerable.Repeat(0, MaxKeyCount).Select(_ => new PatternKey());

		[JsonIgnore]
		public IReadOnlyList<PatternFx> Fx => _fx.Items;
		[JsonProperty(nameof(Fx))]
		readonly ModelList<PatternFx> _fx = new(MakeFx());

		[JsonIgnore]
		public IReadOnlyList<PatternKey> Keys => _keys.Items;
		[JsonProperty(nameof(Keys))]
		readonly ModelList<PatternKey> _keys = new(MakeKeys());

		internal override IEnumerable<Param> ListParams() =>
			Fx.SelectMany(f => f.ListParams()).Concat(
			Keys.SelectMany(k => k.ListParams()));
	}
}