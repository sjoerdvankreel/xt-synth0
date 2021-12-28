using Xt.Synth0.Model;

namespace Xt.Synth0
{
	class AutomationEntry
	{
		internal bool[] Automated { get; }
		internal SynthModel Model { get; } = new SynthModel();
		internal AutomationEntry() => Automated = new bool[Model.AutoParams().Count];
	}
}