using Xt.Synth0.Model;

namespace Xt.Synth0.DSP
{
	class PatternDSP
	{
		internal void Automate(SynthModel synth,
			SequencerModel seq, AudioModel audio, bool[] automated)
		{
			var row = seq.Pattern.Rows[audio.CurrentRow];
			foreach (var fx in row.Fx)
				Automate(synth, fx, automated);
		}

		void Automate(SynthModel model, PatternFx fx, bool[] automated)
		{
			var autos = model.AutoParams();
			var target = fx.Target.Value;
			if (target == 0 || target > autos.Count) return;
			var param = autos[target - 1].Param;
			var value = fx.Value.Value;
			if (value < param.Info.Min) param.Value = param.Info.Min;
			else if (value > param.Info.Max) param.Value = param.Info.Max;
			else param.Value = value;
			automated[target - 1] = true;
		}
	}
}