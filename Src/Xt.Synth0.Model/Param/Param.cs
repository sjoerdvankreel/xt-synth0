using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Xt.Synth0.Model
{
	public class Param : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		static readonly PropertyChangedEventArgs EventArgs = new(nameof(Value));

		[JsonIgnore]
		public ParamInfo Info { get; }
		internal Param(ParamInfo info)
		=> (Info, Value) = (info, info.Default);

		int _value;
		public int Value
		{
			get => _value;
			set
			{
				var comparer = Comparer<int>.Default;
				if (comparer.Compare(value, Info.Min) < 0)
					throw new ArgumentException();
				if (comparer.Compare(value, Info.Max) > 0)
					throw new ArgumentException();
				_value = value;
				PropertyChanged?.Invoke(this, EventArgs);
			}
		}
	}
}