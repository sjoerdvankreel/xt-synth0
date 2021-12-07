using System.Collections.Generic;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class ModelList<T>
		where T : ICopyModel
	{
		readonly IReadOnlyList<T> _items;
		public IReadOnlyList<T> Items => _items;
		internal ModelList(IEnumerable<T> items) => _items = items.ToArray();
	}
}