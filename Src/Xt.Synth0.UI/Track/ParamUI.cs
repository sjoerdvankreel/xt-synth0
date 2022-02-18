using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
    static class ParamUI
    {
        static string Tooltip(SynthModel synth, Param param)
        {
            var result = new StringBuilder();
            result.AppendLine(param.Info.Description);
            var synthParam = synth.SynthParams.SingleOrDefault(p => p.Param == param);
            if (synthParam != null || param.Info.Control == ParamControl.Knob)
                result.AppendLine($"Range: {param.Info.Range(false)}");
            if (synthParam != null)
                result.AppendLine($"Automation target: {synthParam.Index.ToString("X2")}");
            if (param.Info.Control == ParamControl.Knob)
                result.AppendLine("Right-click to set exact value");
            return result.ToString(0, result.Length - Environment.NewLine.Length);
        }

        internal static DockPanel MakeEmpty()
        {
            var result = new DockPanel();
            result.Background = Brushes.Transparent;
            result.HorizontalAlignment = HorizontalAlignment.Stretch;
            return result;
        }

        internal static UIElement Make(
            AppModel app, IUIParamGroupModel group, Param param)
        {
            var result = MakeEmpty();
            result.Add(MakeControl(app, group.ThemeGroup, param), Dock.Left);
            if (param.Info.Control == ParamControl.List) return MakeSubGroup(group, param, result);
            var name = result.Add(Create.Text($"{param.Info.Name} "), Dock.Left);
            name.VerticalAlignment = VerticalAlignment.Center;
            if (param.Info.Control == ParamControl.Toggle) return MakeSubGroup(group, param, result);
            result.Add(MakeValue(param), Dock.Left);
            return MakeSubGroup(group, param, result);
        }

        internal static Control MakeControl(AppModel app, ThemeGroup group, Param param)
        => param.Info.Control switch
        {
            ParamControl.List => MakeList(app, param),
            ParamControl.Toggle => MakeToggle(app, param),
            ParamControl.Knob => MakeKnob(app, group, param),
            _ => throw new InvalidOperationException()
        };

        static TextBlock MakeValue(Param param)
        {
            var result = new TextBlock();
            var binding = Bind.Format(param);
            result.SetBinding(TextBlock.TextProperty, binding);
            result.VerticalAlignment = VerticalAlignment.Center;
            return result;
        }

        static Control MakeToggle(AppModel app, Param param)
        {
            var result = new CheckBox();
            result.ToolTip = Tooltip(app.Track.Synth, param);
            result.SetBinding(ToggleButton.IsCheckedProperty, Bind.To(param));
            return result;
        }

        static Control MakeList(AppModel app, Param param)
        {
            var result = new ComboBox();
            result.SelectedValuePath = nameof(ListItem.Value);
            result.ToolTip = Tooltip(app.Track.Synth, param);
            result.HorizontalAlignment = HorizontalAlignment.Stretch;
            var range = Enumerable.Range(param.Info.Min, param.Info.Max - param.Info.Min + 1);
            var items = range.Select(i => new ListItem(param.Info, i)).ToArray();
            result.SetBinding(Selector.SelectedValueProperty, Bind.To(param));
            result.SetValue(ItemsControl.ItemsSourceProperty, items);
            return result;
        }

        static Control MakeKnob(AppModel app, ThemeGroup group, Param param)
        {
            var result = new Knob();
            result.IsMix = param.Info.IsMix;
            result.Minimum = param.Info.Min;
            result.Maximum = param.Info.Max;
            result.ToolTip = Tooltip(app.Track.Synth, param);
            result.SetBinding(RangeBase.ValueProperty, Bind.To(param));
            result.MouseRightButtonUp += (s, e) => ExactUI.Show(app.Settings, group, param);
            result.Sensitivity = Math.Max(Knob.DefaultSensitivity, param.Info.Max - param.Info.Min);
            return result;
        }

        static Grid MakeSubGroup(IUIParamGroupModel group, Param param, DockPanel content)
        {
            var result = new Grid();
            result.Margin = new(0.0);
            var layer = result.Add(new DockPanel());
            layer.Margin = new(0.0);
            layer.HorizontalAlignment = HorizontalAlignment.Stretch;
            ApplySubGroupBackground(layer, param.Info.SubGroup);
            result.Add(content);
            bool conditional = param.Info.Relevance != null;
            if (conditional) result.SetBinding(UIElement.VisibilityProperty, Bind.Relevance(group, param));
            return result;
        }

        static void ApplySubGroupBackground(DockPanel panel, int subGroup)
        {
            panel.Opacity = subGroup == 0 ? 0.1 : subGroup == 1 ? 0.2 : 0.4;
            if (subGroup == 0) panel.Background = Brushes.White;
            else if (subGroup == 1) panel.SetResourceReference(Panel.BackgroundProperty, Utility.ForegroundKey);
            else if (subGroup == 2) panel.SetResourceReference(Panel.BackgroundProperty, Utility.BackgroundDarkKey);
            else throw new InvalidOperationException();
        }
    }
}