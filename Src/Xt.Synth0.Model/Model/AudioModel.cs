namespace Xt.Synth0.Model
{
	public sealed class AudioModel : ViewModel
	{
		bool _isRunning;
		public bool IsRunning
		{
			get => _isRunning;
			set => Set(ref _isRunning, value);
		}
	}
}