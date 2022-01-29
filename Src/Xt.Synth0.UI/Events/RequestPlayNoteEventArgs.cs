using System;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public class RequestPlayNoteEventArgs : EventArgs
	{
		public int Oct { get; }
		public UnitNote Note { get; }
		internal RequestPlayNoteEventArgs(UnitNote note, int oct)
		=> (Note, Oct) = (note, oct);
	}
}