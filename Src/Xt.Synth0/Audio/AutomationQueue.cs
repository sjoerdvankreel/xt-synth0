using System.Collections.Concurrent;

namespace Xt.Synth0
{
	static class AutomationQueue
	{
		const int MaxActions = 32;
		static readonly AutomationAction[] UIActions = new AutomationAction[MaxActions];
		static readonly AutomationAction[] AudioActions = new AutomationAction[MaxActions];
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

		internal static AutomationAction[] DequeueUI(out int count)
		{
			int i = 0;
			while (UIQueue.TryDequeue(out var action))
				UIActions[i++] = action;
			count = i;
			return UIActions;
		}

		internal static AutomationAction[] DequeueAudio(out int count)
		{
			int i = 0;
			while (AudioQueue.TryDequeue(out var action))
				AudioActions[i++] = action;
			count = i;
			return AudioActions;
		}
	}
}