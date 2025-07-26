using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SkillSystem.Config
{
    [Serializable]
    public class SkillDamageConfig
    {
        
        
        
        
        
        
        [LabelText("触发帧")]
        public int TriggerLogicFrame;
        
        [LabelText("结束帧")]
        public int EndLogicFrame;
        
        [LabelText("触发间隔ms(0 = 一次, > 0 为间隔)")]
        public int TriggerIntervalMs;
        
        [LabelText("跟随特效")]
        public bool FollowEffect;
        
        [LabelText("伤害类型")]
        public DamageType DamageType;
        
        [LabelText("伤害倍率")]
        public int DamageRate;

        
        
        [LabelText("目标类型")]
        public TargetType TargetType;
        

        
        [TitleGroup("技能附加Buff" , "伤害生效的瞬间，附加指定的Buff")]
        [LabelText("附加Buff")]
        public int[] AddBuffIds;
        
        [TitleGroup("触发后续技能" , "造成伤害后且技能释放完毕后，触发指定的技能")]
        [LabelText("触发技能ID")]
        public int TriggerSkillId;
        

        
#if UNITY_EDITOR
        
        

       


        
#endif
        
    }
    
    
    
    

    public enum DamageType
    {
        [LabelText("无伤害")]
        None, 
        [LabelText("物理伤害")]
        ADDamage,
        [LabelText("法术伤害")]
        APDamage,
    }

    public enum DamageDetectionMode
    {
        [LabelText("无配置")]
        None,
        [LabelText("3DBox碰撞检测")]
        Box3D,
        [LabelText("3D球体碰撞检测")]
        Sphere3D,
        [LabelText("3D圆柱体碰撞检测")]
        Cylinder3D,
        [LabelText("半径检测")]
        RadiusDistance,
        [LabelText("所有目标")]
        AllTarget,
    }

    public enum ColliderPosType
    {
        [LabelText("跟随角色朝向")]
        FollowDic,
        [LabelText("跟随角色位置")]
        FollowPos,
        [LabelText("中心坐标")]
        CenterPos,
        [LabelText("目标位置")]
        TargetPos,
        
    }

    public enum TargetType
    {
        [LabelText("无配置")]
        None,
        [LabelText("队友")]
        Teammate,
        [LabelText("敌人")]
        Enemy,
        [LabelText("自身")]
        Self,
        [LabelText("所有对象")]
        All,
    }
}
