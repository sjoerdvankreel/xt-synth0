using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0
{
    static unsafe class AutomationQueue
    {
        const int InitialCapacity = 32;

        static int UIActionsCount = 0;
        static AutomationAction.Native* UIActions;
        static int UIActionsCapacity = InitialCapacity;
        static AutomationQueue() => UIActions = AllocateUIActions(InitialCapacity);
        static AutomationAction.Native* AllocateUIActions(int count)
        => (AutomationAction.Native*)Marshal.AllocHGlobal(Marshal.SizeOf<AutomationAction.Native>() * count);

        static readonly List<AutomationAction.Native> AudioActions = new List<AutomationAction.Native>(InitialCapacity);
        static readonly ConcurrentQueue<AutomationAction.Native> UIQueue = new ConcurrentQueue<AutomationAction.Native>();
        static readonly ConcurrentQueue<AutomationAction.Native> AudioQueue = new ConcurrentQueue<AutomationAction.Native>();

        internal static void EnqueueUI(AutomationAction.Native action) => UIQueue.Enqueue(action);
        internal static void EnqueueAudio(AutomationAction.Native action) => AudioQueue.Enqueue(action);

        internal static void Clear()
        {
            UIQueue.Clear();
            AudioQueue.Clear();
        }

        internal static AutomationAction.Native* DequeueUI(out int count)
        {
            UIActionsCount = 0;
            while (UIQueue.TryDequeue(out var action))
                AddUIAction(action);
            count = UIActionsCount;
            return UIActions;
        }

        internal static IReadOnlyList<AutomationAction.Native> DequeueAudio()
        {
            AudioActions.Clear();
            while (AudioQueue.TryDequeue(out var action))
                AudioActions.Add(action);
            return AudioActions;
        }

        static void AddUIAction(AutomationAction.Native action)
        {
            if (UIActionsCount == UIActionsCapacity)
            {
                UIActionsCapacity *= 2;
                Marshal.FreeHGlobal((IntPtr)UIActions);
                UIActions = AllocateUIActions(UIActionsCapacity);
            }
            UIActions[UIActionsCount] = action;
            UIActionsCount++;
        }
    }
}