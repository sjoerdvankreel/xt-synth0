using System.Collections.Generic;

namespace Xt.Synth0.Model
{
	public sealed class SequencerModel : MainModel
	{
		public PatternModel Pattern { get; } = new();
		public EditModel Edit { get; } = new(nameof(Edit));		
		internal override IEnumerable<SubModel> ListSubModels() 
		=> new SubModel[] { Edit, Pattern };
	}
}