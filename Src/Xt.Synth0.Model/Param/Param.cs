using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Xt.Synth0.Model
{
	internal static class Param
	{
		internal static Param<T> Of<T>(ParamInfo<T> model) => new(model);
	}

	public class Param<T> : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		static readonly PropertyChangedEventArgs EventArgs = new(nameof(Value));

		[JsonIgnore]
		public ParamInfo<T> Info { get; }
		internal Param(ParamInfo<T> info)
		=> (Info, Value) = (info, info.Default);

		T _value;
		public T Value
		{
			get => _value;
			set
			{
				var comparer = Comparer<T>.Default;
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