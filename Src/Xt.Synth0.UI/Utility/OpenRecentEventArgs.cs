namespace Xt.Synth0.UI
{
	public class OpenRecentEventArgs
	{
		public string Path { get; }
		internal OpenRecentEventArgs(string path) => Path = path;
	}
}