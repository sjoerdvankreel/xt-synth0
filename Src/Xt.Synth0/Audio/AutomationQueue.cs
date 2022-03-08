using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Xt.Synth0
{
	static class AutomationQueue
	{
        const int InitialCapacity = 32;
		static readonly List<AutomationAction> UIActions = new List<AutomationAction>(InitialCapacity);
		static readonly List<AutomationAction> AudioActions = new List<AutomationAction>(InitialCapacity);
        static readonly ConcurrentQueue<AutomationAction> UIQueue = new ConcurrentQueue<AutomationAction>();
		static readonly ConcurrentQueue<AutomationAction> AudioQueue = new ConcurrentQueue<AutomationAction>();

		internal static void EnqueueUI(int param, int value)
		=> UIQueue.Enqueue(new AutomationAction(param, value));
		internal static void EnqueueAudio(int param, int value) 
		=> AudioQueue.Enqueue(new AutomationAction(param, value));

		internal static void Clear()
		{
			UIQueue.Clear();
			AudioQueue.Clear();
		}

		internal static IReadOnlyList<AutomationAction> DequeueUI()
		{
            UIActions.Clear();
            while (UIQueue.TryDequeue(out var action))
                UIActions.Add(action);
			return UIActions;
		}

		internal static IReadOnlyList<AutomationAction> DequeueAudio()
		{
            AudioActions.Clear();
            while (AudioQueue.TryDequeue(out var action))
                AudioActions.Add(action);
			return AudioActions;
		}
	}
}