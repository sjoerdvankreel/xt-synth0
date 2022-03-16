using System;

namespace Xt.Synth0
{
	class GCNotification
	{
		internal static void Register(AudioMonitor monitor)
		{
			new GCNotification(monitor, 0);
			new GCNotification(monitor, 1);
			new GCNotification(monitor, 2);
		}

		int _collected;
		readonly int _generation;
		readonly AudioMonitor _monitor;
		GCNotification(AudioMonitor monitor, int generation)
		=> (_monitor, _generation) = (monitor, generation);

		~GCNotification()
		{
			if (_collected != _generation)
			{
				_collected++;
				GC.ReRegisterForFinalize(this);
				return;
			}
            _monitor.OnGCNotification(_generation);
			new GCNotification(_monitor, _generation);
		}
	}
}