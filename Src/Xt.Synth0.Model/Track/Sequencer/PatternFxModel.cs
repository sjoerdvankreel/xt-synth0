using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternFxModel : IParamGroupModel
	{
        public int Index { get; }
        internal PatternFxModel(int index) => Index = index;

        public string Id => "ABD763E7-8A06-4582-8D24-88214BB04A3A";
        public IReadOnlyList<Param> Params => new[] { Target, Value };
        public void* Address(void* parent) => &((PatternRowModel.Native*)parent)->fx[Index * Native.Size];

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal ref struct Native
		{
			internal const int Size = 8;
            internal int value;
            internal int target;
        }

		public Param Value { get; } = new(ValueInfo);
        public Param Target { get; } = new(TargetInfo);
        static readonly ParamInfo ValueInfo = ParamInfo.Pattern(p => &((Native*)p)->value, nameof(Value), "Value", "Automation value", 0, 255, 0);        
		static readonly ParamInfo TargetInfo = ParamInfo.Pattern(p => &((Native*)p)->target, nameof(Target), "Target", "Automation target", 0, 255, 0);
	}
}