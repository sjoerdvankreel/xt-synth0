using System;
using System.ComponentModel;

namespace Xt.Synth0.Model
{
	public sealed class Param : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		static readonly PropertyChangedEventArgs EventArgs = new(nameof(Value));

		public ParamInfo Info { get; }
		internal Param(ParamInfo info) => (Info, Value) = (info, info.Default);

		int _value;
		public int Value
		{
			get => _value;
			set
			{
				if (_value == value) return;
				if (value < Info.Min || value > Info.Max)
					throw new ArgumentException();
				_value = value;
				PropertyChanged?.Invoke(this, EventArgs);
			}
		}
	}
}