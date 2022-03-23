using System.Runtime.InteropServices;

namespace Xt.Synth0
{
	public static class AutomationAction
	{
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct Native
        {
            public int target;
            public int value;

            public Native(int target, int value)
            {
                this.target = target;
                this.value = value;
            }
        }
    }
}