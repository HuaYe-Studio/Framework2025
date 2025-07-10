using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace SkillSystem.Config
{
    [HideMonoScript]
    [Serializable]
    public class SkillCharacterConfig
    {
    
    
        [AssetList]
        [LabelText("角色模型")]
        [PreviewField(70, ObjectFieldAlignment.Center)]
        public GameObject SkillCharacter;

    
        [TitleGroup("技能渲染" , "所有技能渲染数据会在技能释放时触发")]
        [LabelText("技能动画")]
        public AnimationClip AnimationClip;

    
        [BoxGroup("动画数据")]
        [ProgressBar(0,100,r:0,g:255,b:0,Height = 30)]
        [HideLabel]
        [OnValueChanged("OnAnimProgressValueChange")]
        public short animProgess = 0;

        [BoxGroup("动画数据")]
        [LabelText("是否循环动画")]
        public bool IsLoopAnim = false;
    
        [BoxGroup("动画数据")]
        [LabelText("循环次数")]
        [ShowIf("IsLoopAnim")]
        public int LoopCount = 0;
    
        [BoxGroup("动画数据")]
        [LabelText("逻辑帧数")]
        public int LogicFrame = 0;
    
        [BoxGroup("动画数据")]
        [LabelText("动画长度")]
        public float AnimLength = 0;
    
        [BoxGroup("动画数据")]
        [LabelText("动画推荐长度(ms)")]
        public float AnimDuration = 0;
    
    
        private GameObject tempCharacter;
        private bool _isPlaying = false;  //是否正在播放动画
        private double _lastRuntime = 0; //上次运行的时间
    
    

        [GUIColor(0.4f,0.8f,1f)]
        [ButtonGroup("动画控制")]
        [Button("播放" , ButtonSizes.Large)]
        public void Play()
        {
            if (SkillCharacter != null)
            {
                string name = SkillCharacter.name;
                tempCharacter = GameObject.Find(name);
                if (tempCharacter == null)
                {
                    //说明是资源 克隆一个到场景中
                    tempCharacter = GameObject.Instantiate(SkillCharacter);
                }
                
            
                AnimLength = IsLoopAnim ? AnimationClip.length * LoopCount : AnimationClip.length;
                LogicFrame = (int)(IsLoopAnim ? AnimationClip.length / 0.066f * LoopCount : AnimationClip.length / 0.066f);
                AnimDuration = IsLoopAnim ? AnimationClip.length * 1000 * LoopCount : AnimationClip.length * 1000;
                _lastRuntime = 0;
                //开始播放动画
                _isPlaying = true;
                SkillComplierWindow.GetWindow()?.StartPlaySkill();

            }
        }
    
    
        [ButtonGroup("动画控制")]
        [Button("暂停" , ButtonSizes.Large)]
        public void Pause()
        {
            _isPlaying = false;
            SkillComplierWindow.GetWindow()?.PlayPause();
        }

        [GUIColor(0,1,0)]
        [ButtonGroup("动画控制")]
        [Button("保存数据" , ButtonSizes.Large)]
        public void SaveAssets()
        {
            SkillComplierWindow.GetWindow()?.SaveConfig();
        }

        public void OnUpdate(Action callback = null)
        {
            //模拟Update更新动画

            if (_isPlaying)
            {
                if (_lastRuntime == 0)
                {
                    _lastRuntime = EditorApplication.timeSinceStartup;
                }
                //当前运行时间
                double currentTime = EditorApplication.timeSinceStartup - _lastRuntime;

                //动画播放进度
                float curAniNormalizationValue = (float)currentTime / AnimLength;
                animProgess = (short)Mathf.Clamp(curAniNormalizationValue * 100 ,0,100);   
            
                //计算逻辑帧
                LogicFrame = (int)(currentTime / LogicFrameConfig.LogicFrameInterval);
            
                //动画采样
                AnimationClip.SampleAnimation(tempCharacter , (float)currentTime);

                if (animProgess == 100)
                {
                    //播放完成
                    OnPlayOver();
                }
            
                callback?.Invoke();
            }
        
        
        }

        public void OnPlayOver()
        {
            _isPlaying = false;
            SkillComplierWindow.GetWindow()?.PlaySkillEnd();
        }

        public void OnAnimProgressValueChange(float value)
        {
            if (SkillCharacter != null)
            {
                string name = SkillCharacter.name;
                tempCharacter = GameObject.Find(name);
                if (tempCharacter == null)
                {
                    //说明是资源 克隆一个到场景中
                    tempCharacter = GameObject.Instantiate(SkillCharacter);
                }

                
                //根据当前进度进行采样
                float progressValue = (value / 100) * AnimationClip.length;
                LogicFrame = (int)(progressValue / LogicFrameConfig.LogicFrameInterval);
                AnimationClip.SampleAnimation(tempCharacter, progressValue);
            
            
            }
        }

    }
}
