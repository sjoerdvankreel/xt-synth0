using System.Runtime.InteropServices;

namespace Xt.Synth0
{
	public static class AutomationAction
	{
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct Native
        {
            public int automationId;
            public int paramIndex;
            public int paramValue;
            int pad__;

            public Native(int automationId, int paramIndex, int paramValue)
            {
                this.automationId = automationId;
                this.paramIndex = paramIndex;
                this.paramValue = paramValue;
                this.pad__ = 0;
            }
        }
    }
}