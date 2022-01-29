using System.Windows;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public class AmpBox : HexBox
	{
		public static readonly DependencyProperty NoteProperty = DependencyProperty.Register(
			nameof(Note), typeof(int), typeof(AmpBox), new(0, OnNoteChanged));
		public static int GetNote(DependencyObject obj) => (int)obj.GetValue(NoteProperty);
		public static void SetNote(DependencyObject obj, int value) => obj.SetValue(NoteProperty, value);

		static void OnNoteChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) => ((AmpBox)obj).Reformat();

		public int Note
		{
			get => GetNote(this);
			set => SetNote(this, value);
		}

		protected override void Reformat()
		{
			if (Note >= (int)PatternNote.C)
				base.Reformat();
			else
				HexValue = PatternKey.Notes[Note];
		}
	}
}