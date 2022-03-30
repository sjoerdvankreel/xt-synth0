﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public unsafe sealed class SequencerModel : MainModel
    {
        public override int Index => 0;
        public override string Id => "2468A725-5A55-4305-A438-C3A70DD3054F";
        public override IReadOnlyList<IParamGroupModel> Groups => new[] { Edit };
        public override IReadOnlyList<IGroupContainerModel> Children => new[] { Pattern };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public ref struct Native
        {
            internal EditModel.Native edit;
            internal PatternModel.Native pattern;
        }

        public EditModel Edit { get; } = new();
        public PatternModel Pattern { get; } = new();
        public ControlModel Control { get; } = new();
        public MonitorModel Monitor { get; } = new();

        public void ToNative(Native* native) => ToNative(this, native);
        void ToNative(IGroupContainerModel container, void* native)
        {
            for (int i = 0; i < container.Children.Count; i++)
                ToNative(container.Children[i], container.Children[i].Address(native));
            for (int i = 0; i < container.Groups.Count; i++)
                for (int j = 0; j < container.Groups[i].Params.Count; j++)
                {
                    var param = container.Groups[i].Params[j];
                    *param.Info.Address(container.Groups[i].Address(native)) = param.Value;
                }
        }

        public void Paste(int pattern, int? key, int? fx, SequencerClipboardData data)
        {
            int count = Edit.Rows.Value;
            int start = pattern * count;
            if (key != null && fx != null) throw new InvalidOperationException();
            if (fx != null && data.Type == SequencerClipboardType.Fx)
                for (int i = start; i < count; i++)
                    data.Fx[i - start].CopyTo(Pattern.Rows[i].Fx[fx.Value]);
            else if (key != null && data.Type == SequencerClipboardType.Keys)
                for (int i = start; i < count; i++)
                    data.Keys[i - start].CopyTo(Pattern.Rows[i].Keys[fx.Value]);
            else if(data.Type == SequencerClipboardType.Rows)
                for (int i = start; i < count; i++)
                    data.Rows[i - start].CopyTo(Pattern.Rows[i]);
        }

        public SequencerClipboardData Copy(int pattern, int? key, int? fx)
        {
            int count = Edit.Rows.Value;
            int start = pattern * count;
            if (key != null && fx != null) throw new InvalidOperationException();
            PatternRowModel[] rows = Pattern.Rows.Skip(start).Take(count).ToArray();
            if (key != null) return new SequencerClipboardData(rows.Select(r => r.Keys[key.Value].Copy()).ToArray());
            else if (fx != null) return new SequencerClipboardData(rows.Select(r => r.Fx[fx.Value].Copy()).ToArray());
            else return new SequencerClipboardData(rows.Select(r => r.Copy()).ToArray());
        }
    }
}