using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xt.Synth0.Model
{
    public unsafe abstract class AutomatableModel : MainModel
    {
        public override int Index => 0;
        public IReadOnlyList<AutoParam> AutoParams { get; }
        public override IReadOnlyList<IGroupContainerModel> Children => new IGroupContainerModel[0];

        protected AutomatableModel()
        {
            var @params = ListParams(this).Select((p, i) => new AutoParam((IAutomationGroupModel)p.Group, i + 1, p.Param));
            AutoParams = new ReadOnlyCollection<AutoParam>(@params.ToArray());
        }

        public void ToNative(ParamBinding.Native* binding)
        {
            for (int i = 0; i < Params.Count; i++)
                *binding->@params[i] = Params[i].Value;
        }

        public void FromNative(ParamBinding.Native* binding)
        {
            for (int i = 0; i < Params.Count; i++)
                Params[i].Value = *binding->@params[i];
        }

        public void Bind(void* native, ParamBinding.Native* binding)
        {
            var @params = binding->@params;
            for (int p = 0; p < Params.Count; p++)
                @params[p] = Params[p].Info.Address(AutoParams[p].Group.Address(native));
        }

        public ParamInfo.Native[] ParamInfos()
        {
            var result = new ParamInfo.Native[AutoParams.Count];
            for (int i = 0; i < AutoParams.Count; i++)
            {
                result[i].min = Params[i].Info.Min;
                result[i].max = Params[i].Info.Max;
                result[i].realtime = Params[i].Info.Realtime ? 1 : 0;
            }
            return result;
        }
    }
}