using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Xt.Synth0
{
	static class AutomationPool
	{
		const int MaxPoolSize = 128;
		static IEnumerable<AutomationEntry> Create()
		=> Enumerable.Range(0, MaxPoolSize).Select(_ => new AutomationEntry());
		static readonly ConcurrentBag<AutomationEntry> _entries = new(Create());

		internal static void Return(AutomationEntry entry)
		=> _entries.Add(entry);
		internal static AutomationEntry Get()
		=> _entries.TryTake(out var result) ? result : throw new InvalidOperationException();
	}
}