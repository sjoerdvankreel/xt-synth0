using System;

namespace Xt.Synth0.UI
{
	public class OpenRecentEventArgs : EventArgs
	{
		public string Path { get; }
		internal OpenRecentEventArgs(string path) => Path = path;
	}
}