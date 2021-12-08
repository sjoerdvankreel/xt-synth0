﻿using System.Windows;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	class PatternSelector : MultiConverter<bool, int, int, UIElement>
	{
		readonly UIElement[] _elements;
		internal PatternSelector(UIElement[] elements) 
		=> _elements = elements;

		protected override UIElement Convert(bool running, int edit, int row)
		{
			if (!running) return _elements[edit - 1];
			return _elements[row / PatternModel.PatternRows];
		}
	}
}