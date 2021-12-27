using System;

namespace Xt.Synth0
{
	class GCNotification
	{
		internal static void Register(AudioEngine engine)
		{
			new GCNotification(engine, 0);
			new GCNotification(engine, 1);
			new GCNotification(engine, 2);
		}

		int _collected;
		readonly int _generation;
		readonly AudioEngine _engine;
		GCNotification(AudioEngine engine, int generation)
		=> (_engine, _generation) = (engine, generation);

		~GCNotification()
		{
			if (_collected != _generation)
			{
				_collected++;
				GC.ReRegisterForFinalize(this);
				return;
			}
			_engine.OnGCNotification(_generation);
			new GCNotification(_engine, _generation);
		}
	}
}