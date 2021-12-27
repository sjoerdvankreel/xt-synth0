namespace Xt.Synth0.UI
{
	class WarningFormatter : MultiConverter<bool, bool, string>
	{
		protected override string Convert(bool clip, bool overload)
		{
			if (!clip && !overload) return "None";
			if (!clip) return "Overload";
			if (!overload) return "Clip";
			return "Clip/Overload";
		}
	}
}