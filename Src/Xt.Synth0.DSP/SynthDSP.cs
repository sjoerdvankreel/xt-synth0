using Xt.Synth0.Model;

namespace Xt.Synth0.DSP
{
	class SynthDSP
	{
		readonly UnitDSP[] _units = new UnitDSP[SynthModel.UnitCount];

		internal SynthDSP()
		{
			for (int u = 0; u < _units.Length; u++)
				_units[u] = new UnitDSP();
		}

		internal void Reset()
		{
			foreach (var unit in _units)
				unit.Reset();
		}

		public float Next(SynthModel synth, float rate)
		{
			float result = 0.0f;
			for (int u = 0; u < _units.Length; u++)
				result += _units[u].Next(synth.Global, synth.Units[u], rate);
			return result * synth.Amp.Lvl.Value / 255.0f;
		}
	}
}