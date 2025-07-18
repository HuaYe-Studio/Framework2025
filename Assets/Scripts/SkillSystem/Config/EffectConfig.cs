using System;
using Sirenix.OdinInspector;
using SkillSystem.Agent;
using UnityEngine;

namespace SkillSystem.Config
{
    [HideMonoScript]
    [Serializable]
    public class EffectConfig
    {
    
        [AssetList]
        [LabelText("技能特效")]
        [PreviewField(70 , ObjectFieldAlignment.Left)]
        public GameObject Effect;

        [LabelText("触发帧")]
        public int TriggerFrame;
        
        [LabelText("结束帧")]
        public int EndFrame;
        
        [LabelText("位置偏移")]
        public Vector3 EffectOffset;
        
        [LabelText("特效位置类型")]
        public EffectType EffectType;
        
        [ToggleGroup("ShowTransParentType" , "是否设置父节点特效")]
        public bool ShowTransParentType = false;
        
        [ToggleGroup("ShowTransParentType" , "节点类型")]
        public TransParentType TransParentType;


        // 特效缓存
        public GameObject GameEffectCache;


#if UNITY_EDITOR
        
        private GameObject _cloneEffect;
        private AnimationAgent _animationAgent;
        private ParticleAgent _particleAgent;
        
        
        private int _currentLogicFrame = 0;

        public void StartPlaySkill()
        {
            DestroyEffect();

            _currentLogicFrame = 0;
        }

        public void SkillPlayPause()
        {
            DestroyEffect();
        }

        public void PlaySkillEnd()
        {
            DestroyEffect();
        }

        public void OnLogicFrameUpdate()
        {
            if (_currentLogicFrame == TriggerFrame)
            {
                CreateEffect();
            }
            else if(_currentLogicFrame == EndFrame)
            {
                DestroyEffect();
            }
            
            
            _currentLogicFrame++;
            
        }

        public void CreateEffect()
        {
            if (Effect != null)
            {
                _cloneEffect = GameObject.Instantiate(Effect);
                _cloneEffect.transform.position = SkillComplierWindow.GetCharacterPosition();
                //代理释放动画或粒子
                _animationAgent = new AnimationAgent();
                _animationAgent.InitPlayAnim(_cloneEffect.transform);
                
                _particleAgent = new ParticleAgent();
                _particleAgent.InitPlayAnim(_cloneEffect.transform);
            }
        }

        public void DestroyEffect()
        {
            if (_cloneEffect != null)
            {
                GameObject.DestroyImmediate(_cloneEffect);
            }

            if (_particleAgent != null)
            {
                _particleAgent.OnDestroy();
                _particleAgent = null;
            }

            if (_animationAgent != null)
            {
                _animationAgent.OnDestroy();
                _animationAgent = null;
            }
        }
        
        
    
#endif
        
        

    }
    
    

    
    
    
    public enum TransParentType
    {
        [LabelText("无配置")]
        None,
        [LabelText("左手")]
        LeftHand,
        [LabelText("右手")]
        RightHand,
    }

    
    public enum EffectType
    {
        [LabelText("跟随角色位置和方向")]
        FollowPosDir,
        [LabelText("只跟随角色方向")]
        FollowDir,
        [LabelText("屏幕中心位置")]
        CenterPos,
        [LabelText("引导位置")]
        GuidePos,
        [LabelText("跟随特效移动位置")]
        FollowEffectMovePos,
    }
}