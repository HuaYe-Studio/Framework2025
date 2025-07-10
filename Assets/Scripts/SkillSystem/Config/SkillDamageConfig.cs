using System;
using FixIntPhysics;
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

        [LabelText("检测模式")]
        [OnValueChanged("OnDectionValueChange")]
        public DamageDetectionMode DamageDetectionMode;
        
        [LabelText("Box碰撞盒大小")]
        [ShowIf("_showBox3D")]
        [OnValueChanged("OnBoxSizeValueChange")]
        public Vector3 BoxSize = new Vector3(1, 1, 1);
        
        [LabelText("Box碰撞盒偏移")]
        [ShowIf("_showBox3D")]
        [OnValueChanged("OnOffsetValueChange")]
        public Vector3 BoxOffset = new Vector3(0, 0, 0);
        
        [LabelText("球形碰撞盒偏移")]
        [ShowIf("_showSphere3D")]
        [OnValueChanged("OnOffsetValueChange")]
        public Vector3 SphereOffset = new Vector3(0, 0.9f, 0);
        
        [LabelText("球形碰撞盒半径")]
        [ShowIf("_showSphere3D")]
        [OnValueChanged("OnSphereSizeValueChange")]
        public float Radius = 1;
        
        [LabelText("球形检测半径高度")]
        [ShowIf("_showSphere3D")]
        public float RadiusHeight = 0;
        
        [LabelText("检测位置类型")]
        public ColliderPosType ColliderPosType = ColliderPosType.FollowDic;
        
        [LabelText("目标类型")]
        public TargetType TargetType;
        
        [TitleGroup("技能附加Buff" , "伤害生效的瞬间，附加指定的Buff")]
        [LabelText("附加Buff")]
        public int[] AddBuffIds;
        
        [TitleGroup("触发后续技能" , "造成伤害后且技能释放完毕后，触发指定的技能")]
        [LabelText("触发技能ID")]
        public int TriggerSkillId;


        
#if UNITY_EDITOR
        
        
        private bool _showBox3D;
        private bool _showSphere3D;
        
        //定点数学物理库
        private FixIntBoxCollider _boxCollider;
        private FixIntSphereCollider _sphereCollider;
        
        private int _curLogicFrame = 0;
        


        private void OnDectionValueChange(DamageDetectionMode mode)
        {
            _showBox3D = mode == DamageDetectionMode.Box3D;
            _showSphere3D = mode == DamageDetectionMode.Sphere3D;
            CreateCollider();
        }

        private void CreateCollider()
        {
            DestroyCollider();
            switch (DamageDetectionMode)
            {
                case DamageDetectionMode.Box3D:
                    _boxCollider = new FixIntBoxCollider(BoxSize, SkillComplierWindow.GetCharacterPosition() + BoxOffset);
                    _boxCollider.SetBoxData(SkillComplierWindow.GetCharacterPosition() + BoxOffset , BoxSize , ColliderPosType == ColliderPosType.FollowPos);
                    break;
                case DamageDetectionMode.Sphere3D:
                    _sphereCollider = new FixIntSphereCollider(Radius , SkillComplierWindow.GetCharacterPosition() + SphereOffset);
                    _sphereCollider.SetBoxData(Radius , SkillComplierWindow.GetCharacterPosition() + SphereOffset , ColliderPosType == ColliderPosType.FollowPos);
                    break;
            }            
            
            
            
        }

        private void DestroyCollider()
        {
            if(_boxCollider != null) _boxCollider.OnRelease();
            if(_sphereCollider != null) _sphereCollider.OnRelease();
        }

        public void OnBoxSizeValueChange(Vector3 size)
        {
            if (_boxCollider!= null)
            {
                _boxCollider.SetBoxData(SkillComplierWindow.GetCharacterPosition() + BoxOffset, size, ColliderPosType == ColliderPosType.FollowPos);
            }
        }

        public void OnSphereSizeValueChange(float size)
        {
            if (_sphereCollider != null)
            {
                _sphereCollider.SetBoxData(size, SkillComplierWindow.GetCharacterPosition() + SphereOffset, ColliderPosType == ColliderPosType.FollowPos);
            }
        }

        public void OnOffsetValueChange(Vector3 offset)
        {
            switch (DamageDetectionMode)
            {
                case DamageDetectionMode.Box3D:
                    if(_boxCollider == null) return;
                    _boxCollider.SetBoxData(SkillComplierWindow.GetCharacterPosition() + offset, BoxSize, ColliderPosType == ColliderPosType.FollowPos);
                    break;
                case DamageDetectionMode.Sphere3D:
                    if (_sphereCollider == null) return;
                    _sphereCollider.SetBoxData(Radius, SkillComplierWindow.GetCharacterPosition() + offset, ColliderPosType == ColliderPosType.FollowPos);
                    break;
            }
        }

        public void OnInit()
        {
            CreateCollider();
        }

        public void OnRelease()
        {
            DestroyCollider();
        }

        public void StartPlaySkill()
        {
            _curLogicFrame = 0;
            DestroyCollider();
        }

        public void PlaySkillEnd()
        {
            DestroyCollider();
        }

        public void UpdateLogic()
        {

            if (_curLogicFrame == TriggerLogicFrame)
            {
                CreateCollider();
            }
            else if (_curLogicFrame == EndLogicFrame)
            {
                DestroyCollider();
            }
            
            
            _curLogicFrame++;
            
        }


        
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
