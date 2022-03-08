using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xt.Synth0.Model
{
	public unsafe abstract class MainModel : IGroupContainerModel
	{
        bool _updating = false;
		protected IReadOnlyList<Param> Params { get; }
		public event EventHandler<ParamChangedEventArgs> ParamChanged;

		public abstract int Index { get; }
		public abstract string Id { get; }
		public abstract IReadOnlyList<IParamGroupModel> Groups { get; }
		public abstract IReadOnlyList<IGroupContainerModel> Children { get; }
		public void* Address(void* parent) => throw new NotSupportedException();

        public int ParamCount => Params.Count;
        public void BeginUpdate() => _updating = true;
        public int GetParam(int index) => Params[index].Value;

		protected MainModel()
		{
			Params = new ReadOnlyCollection<Param>(ListParams(this).Select(p => p.Param).ToArray());
			for (int p = 0; p < Params.Count; p++)
			{
				var args = new ParamChangedEventArgs(p);
				Params[p].PropertyChanged += (s, e) => ParamChanged?.Invoke(this, args);
			}
        }

        public void SetParam(int index, int value)
        {
            if (!_updating) throw new InvalidOperationException();
            Params[index].SetValue(value);
        }

        public void EndUpdate()
        {
            _updating = false;
            ParamChanged?.Invoke(this, new ParamChangedEventArgs(-1));
        }

        public void CopyTo(MainModel main)
        {
            main.BeginUpdate();
            try
            {
                for (int p = 0; p < ParamCount; p++)
                    main.SetParam(p, GetParam(p));
            }
            finally
            {
                main.EndUpdate();
            }
        }

        protected IList<(IParamGroupModel Group, Param Param)> ListParams(IGroupContainerModel container)
		{
			var result = new List<(IParamGroupModel, Param)>();
			for (int i = 0; i < container.Groups.Count; i++)
				for (int j = 0; j < container.Groups[i].Params.Count; j++)
					result.Add((container.Groups[i], container.Groups[i].Params[j]));
			for (int i = 0; i < container.Children.Count; i++)
				result.AddRange(ListParams(container.Children[i]));
			return result;
		}
	}
}