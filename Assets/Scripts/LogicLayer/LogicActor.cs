using RenderLayer;
using UnityEngine;

namespace LogicLayer
{
    public partial class LogicActor : LogicObject
    {
        public override void OnCreate()
        {
            InitActorSkill();
        }

        public override void OnLogicFrameUpdate()
        {
            OnLogicFrameUpdateGravity();
            OnLogicFrameUpdateSkill();
            OnLoigcFrameUpdateMove();
        }

        public override void OnDestroy()
        {
            
        }

        public void PlayAnim(AnimationClip clip)
        {
            RenderObject.PlayAnim(clip);
        }

        public void PlayAnim(string clipName)
        {
            RenderObject.PlayAnim(clipName);
        }

        public void SetRenderData(RenderObject renderObject)
        {
            RenderObject = renderObject;
        }
    }
}
