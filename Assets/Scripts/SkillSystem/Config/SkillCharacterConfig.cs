using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SkillSystem.Config
{
    [Serializable]
    [HideMonoScript]
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
        public short animProgess = 0;
    }
}