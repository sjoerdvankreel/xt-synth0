using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;

namespace Xt.Synth0
{
	class NoAutomationPeer : FrameworkElementAutomationPeer
	{
		readonly List<AutomationPeer> _children = new List<AutomationPeer>();
		internal NoAutomationPeer(FrameworkElement owner) : base(owner) { }
		protected override string GetNameCore() => nameof(NoAutomationPeer);
		protected override List<AutomationPeer> GetChildrenCore() => _children;
		protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Window;
	}
}