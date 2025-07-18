using FixMath;
using LogicLayer;
using RenderLayer;
using SkillSystem.Config;

namespace SkillSystem.Runtime.Logic
{
    public class SkillEffectLogic : LogicObject
    {
        
        private LogicActor _actor;
        private EffectConfig _effectConfig;
        
        public SkillEffectLogic(LogicObjectType logicType, EffectConfig effectConfig, RenderObject renderObject,
            LogicActor owner)
        {
            ObjectType = logicType;
            RenderObject = renderObject;
            _actor = owner;
            _effectConfig = effectConfig;
            this.LogicAxis = owner.LogicAxis;
            switch (effectConfig.EffectType)
            {
                case EffectType.FollowDir:
                case EffectType.FollowPosDir:
                    FixIntVector3 pos = new FixIntVector3(effectConfig.EffectOffset) * LogicAxis;
                    pos.y = FixIntMath.Abs(pos.y);
                    LogicPos = _actor.LogicPos + pos;
                    break;
                    
                    
            }
            
            
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RenderObject.OnRelease();
        }
    }
}