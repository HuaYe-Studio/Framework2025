using BuffSystem.BuffDesign;
using UnityEngine;

namespace BuffSystem.BuffCallbackImpl
{
    public sealed class CastBuff2Target : BuffCallback
    {
        public readonly string BuffId;

        public CastBuff2Target(string buffId)
        {
            BuffId = buffId;
        }

        public override void Apply(BuffRunTimeInfo info, params object[] customParams)
        {
            var target = info.Target;
            var buffManager = target.GetComponent<BuffManager>();
            buffManager.AddBuff(BuffId, info.Creator);
            Debug.Log($"{info.BuffData.Id} 施加了一个Buff {BuffId}");
        }
    }
}