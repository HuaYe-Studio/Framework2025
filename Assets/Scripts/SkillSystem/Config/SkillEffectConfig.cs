using System;
using Sirenix.OdinInspector;
using SkillSystem.Agent;
using UnityEngine;

namespace SkillSystem.Config
{
    [HideMonoScript]
    [Serializable]
    public class SkillEffectConfig
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
        [OnValueChanged("OnPositionChange")]
        public Vector3 EffectOffset;
        
        [LabelText("旋转偏移")]
        [OnValueChanged("OnRotationChange")]
        public Vector3 EffectRotate;
        
        [LabelText("特效位置类型")]
        public EffectType EffectType;
        
        [ToggleGroup("ShowTransParentType" , "是否设置父节点特效")]
        public bool ShowTransParentType = false;
        
        [ToggleGroup("ShowTransParentType" , "节点类型")]
        public TransParentType TransParentType;



#if UNITY_EDITOR
        
        private GameObject _cloneEffect;
        private AnimationAgent _animationAgent;
        private ParticleAgent _particleAgent;
        
        
        // private int _currentLogicFrame = 0;
        private int _currentTimeMS = 0;
        private bool _isTrigger = false;
        private bool _isEnd = false;

        public void StartPlaySkill()
        {
            DestroyEffect();

            // _currentLogicFrame = 0;
            _currentTimeMS = 0;
            _isTrigger = false;
            _isEnd = false;
        }

        public void SkillPlayPause()
        {
            DestroyEffect();
        }

        public void PlaySkillEnd()
        {
            DestroyEffect();
        }

        // public void OnLogicFrameUpdate()
        // {
        //     if (_currentLogicFrame == TriggerFrame)
        //     {
        //         CreateEffect();
        //     }
        //     else if(_currentLogicFrame == EndFrame)
        //     {
        //         DestroyEffect();
        //     }
        //     
        //     
        //     _currentLogicFrame++;
        //     
        // }

        public void OnUpdate(int currentTimeMS)
        {
            _currentTimeMS = currentTimeMS;
            if (_currentTimeMS >= TriggerFrame && !_isTrigger)
            {
                _isTrigger = true;
                CreateEffect(true);
            }
            else if (_currentTimeMS >= EndFrame && !_isEnd)
            {
                _isEnd = true;
                DestroyEffect();
            }
        }

        public void CreateEffect(bool isPlay = false)
        {
            if (Effect != null)
            {
                _cloneEffect = GameObject.Instantiate(Effect);
                // _cloneEffect.transform.position = SkillComplierWindow.GetCharacterPosition().position + EffectOffset;
                //模型空间位置
                _cloneEffect.transform.position = SkillComplierWindow.GetCharacterTransform().position + 
                                                  SkillComplierWindow.GetCharacterTransform().forward * EffectOffset.z + 
                                                  SkillComplierWindow.GetCharacterTransform().right * EffectOffset.x + 
                                                  SkillComplierWindow.GetCharacterTransform().up * EffectOffset.y;
                
                _cloneEffect.transform.localRotation = Quaternion.Euler(EffectRotate);
                //代理释放动画或粒子
                _animationAgent = new AnimationAgent();
                _animationAgent.InitPlayAnim(_cloneEffect.transform);
                
                _particleAgent = new ParticleAgent();
                _particleAgent.InitPlayAnim(_cloneEffect.transform);
                if (isPlay)
                {
                    _animationAgent.IsPlaying = true;
                    _particleAgent.IsPlaying = true;
                }
            }
        }

        public void OnPositionChange(Vector3 position)
        {
            if (_cloneEffect != null)
            {
                _cloneEffect.transform.position = SkillComplierWindow.GetCharacterTransform().position + 
                                                  SkillComplierWindow.GetCharacterTransform().forward * position.z + 
                                                  SkillComplierWindow.GetCharacterTransform().right * position.x + 
                                                  SkillComplierWindow.GetCharacterTransform().up * position.y;
            }
        }

        public void OnRotationChange(Vector3 rotation)
        {
            if (_cloneEffect != null)
            {
                _cloneEffect.transform.localRotation = Quaternion.Euler(rotation);
            }
        }

        public void OnSimulate(float deltaTime)
        {
            if (_animationAgent != null)
            {
                _animationAgent.OnSimulate(deltaTime);
            }

            if (_particleAgent != null)
            {
                _particleAgent.OnSimulate(deltaTime);
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