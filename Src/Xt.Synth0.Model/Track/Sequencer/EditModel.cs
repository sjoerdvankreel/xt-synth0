using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public sealed class EditModel : GroupModel
	{
		public const int NativeSize = 1;

		[StructLayout(LayoutKind.Sequential)]
		internal struct Native
		{
			internal int fx;
			internal int act;
			internal int pats;
			internal int keys;
		}

		static readonly ParamInfo FxInfo = new DiscreteInfo(
			nameof(Fx), "Effect count", 0, PatternRow.MaxFxCount, 1);
		static readonly ParamInfo KeysInfo = new DiscreteInfo(
			nameof(Keys), "Note count", 1, PatternRow.MaxKeyCount, 2);
		static readonly ParamInfo PatsInfo = new DiscreteInfo(
			nameof(Pats), "Pattern count", 1, PatternModel.PatternCount, 1);
		static readonly ParamInfo ActInfo = new DiscreteInfo(
			nameof(Act), "Active pattern", 1, PatternModel.PatternCount, 1);

		public Param Fx { get; } = new(FxInfo);
		public Param Act { get; } = new(ActInfo);
		public Param Pats { get; } = new(PatsInfo);
		public Param Keys { get; } = new(KeysInfo);

		internal EditModel(string name) : base(name) { }
		internal override Param[][] ListParamGroups() => new[] {
			new [] { Keys, Fx },
			new [] { Pats, Act }
		};

		internal unsafe void ToNative(Native* native)
		{
			native->fx = Fx.Value;
			native->act = Act.Value;
			native->pats = Pats.Value;
			native->keys = Keys.Value;
		}

		internal unsafe void FromNative(Native* native)
		{
			Fx.Value = native->fx;
			Act.Value = native->act;
			Pats.Value = native->pats;
			Keys.Value = native->keys;
		}
	}
}