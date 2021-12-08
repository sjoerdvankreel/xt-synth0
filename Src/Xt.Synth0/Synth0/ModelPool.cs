using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
	static class ModelPool
	{
		const int MaxPoolSize = 128;
		static IEnumerable<SynthModel> Create()
		=> Enumerable.Range(0, MaxPoolSize).Select(_ => new SynthModel());
		static readonly ConcurrentBag<SynthModel> _models = new(Create());

		internal static SynthModel CurrentModel { get; set; }
		internal static DispatcherOperation CurrentCopyOperation { get; set; }

		internal static void Return(SynthModel model)
		=> _models.Add(model);
		internal static SynthModel Get()
		=> _models.TryTake(out var result) ? result : throw new InvalidOperationException();
	}
}