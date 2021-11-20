using System.Collections.ObjectModel;
using System.Linq;

namespace Xt.Synth0.Model
{
	public class TrackModel : GroupModel<TrackModel>
	{
		const int Length = 64;

		public ReadOnlyCollection<NoteModel> Notes { get; }
			= new ReadOnlyCollection<NoteModel>(Enumerable
				.Range(0, Length).Select(_ => new NoteModel()).ToList());

		public override Param<bool>[] BoolParams() => new Param<bool>[0];
		public override Param<int>[] IntParams() => Notes.SelectMany(n => n.IntParams()).ToArray();
	}
}