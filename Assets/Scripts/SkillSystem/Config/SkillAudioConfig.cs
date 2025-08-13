using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SkillSystem.Config
{
    [Serializable]
    public class SkillAudioConfig
    {
        [AssetList]
        [BoxGroup("音效文件")]
        [PreviewField(70 , ObjectFieldAlignment.Left)]
        [OnValueChanged("OnAudioChange")]
        public AudioClip SkillAudio;
        
        [LabelText("音效文件名称")]
        [BoxGroup("音效文件")]
        [ReadOnly]
        [GUIColor("green")]
        public string SkillAudioName;
        
        [BoxGroup("音效参数配置")]
        [LabelText("触发帧")]
        [GUIColor("green")]
        public int TriggerFrame;
        
        [ToggleGroup("isLoop" , "是否循环")]
        public bool isLoop = false;
        
        [ToggleGroup("isLoop" , "结束帧")]
        [LabelText("结束帧")]
        public int EndFrame;



        /// <summary>
        /// 音效文件发生变化时，更新音效文件名称
        /// </summary>
        public void OnAudioChange()
        {
            if (SkillAudio != null)
            {
                SkillAudioName = SkillAudio.name;
            }
        }
        
    }
}