using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Xt.Synth0.Model
{
    public unsafe abstract class MainModel : IGroupContainerModel
    {
        bool _updated = false;
        int? _updatingThread = null;
        protected IReadOnlyList<Param> Params { get; }
        public event EventHandler<ParamChangedEventArgs> ParamChanged;

        public abstract int Index { get; }
        public abstract string Id { get; }
        public abstract IReadOnlyList<IParamGroupModel> Groups { get; }
        public abstract IReadOnlyList<IGroupContainerModel> Children { get; }
        public void* Address(void* parent) => throw new NotSupportedException();

        public int ParamCount => Params.Count;
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

        public void BeginUpdate()
        {
            if (_updatingThread != null) throw new InvalidOperationException();
            _updatingThread = Thread.CurrentThread.ManagedThreadId;
        }

        public void EndUpdate()
        {
            if (_updated) ParamChanged?.Invoke(this, new ParamChangedEventArgs(-1));
            _updatingThread = null;
            _updated = false;
        }

        public void SetParam(int index, int value)
        {
            if (_updatingThread != Thread.CurrentThread.ManagedThreadId) throw new InvalidOperationException();
            _updated |= Params[index].SetValue(value);
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