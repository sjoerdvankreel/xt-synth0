using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public unsafe sealed class PatternRowModel : IGroupContainerModel
    {
        public int Index { get; }
        internal PatternRowModel(int index) => Index = index;

        public string Id => "5E1E96A6-1210-4FC4-BB3F-042C854935A5";
        public IReadOnlyList<IGroupContainerModel> Children => new IGroupContainerModel[0];
        public IReadOnlyList<IParamGroupModel> Groups => Fx.Concat<IParamGroupModel>(Keys).ToArray();
        public void* Address(void* parent) => &((PatternModel.Native*)parent)->rows[Index * Native.Size];

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal const int Size = 88;
            internal fixed byte fx[SharedConfig.MaxFxs * PatternFxModel.Native.Size];
            internal fixed byte keys[SharedConfig.MaxKeys * PatternKeyModel.Native.Size];
        }

        public IReadOnlyList<PatternFxModel> Fx = new ReadOnlyCollection<PatternFxModel>(MakeFx());
        public IReadOnlyList<PatternKeyModel> Keys = new ReadOnlyCollection<PatternKeyModel>(MakeKeys());
        static IList<PatternFxModel> MakeFx() => Enumerable.Range(0, SharedConfig.MaxFxs).Select(i => new PatternFxModel(i)).ToList();
        static IList<PatternKeyModel> MakeKeys() => Enumerable.Range(0, SharedConfig.MaxKeys).Select(i => new PatternKeyModel(i)).ToList();
    }
}