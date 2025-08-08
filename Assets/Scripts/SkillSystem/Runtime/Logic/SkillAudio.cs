namespace SkillSystem.Runtime
{
    public partial class Skill
    {



        /// <summary>
        /// 音效逻辑帧更新
        /// </summary>
        public void OnLogicFrameUpdateAudio()
        {
            if (_skillDataConfig.AudioConfigs is { Count: > 0 })
            {
                _skillDataConfig.AudioConfigs.ForEach(audioConfig =>
                {
                    if (audioConfig.TriggerFrame == _currentLogicTime)
                    {
                        //播放音效
                        AudioController.GetInstance().PlaySoundByAudioClip(audioConfig.SkillAudio , audioConfig.isLoop , 100);
                    }

                    if (audioConfig.isLoop && audioConfig.EndFrame == _currentLogicTime)
                    {
                        //销毁音效
                        AudioController.GetInstance().StopSound(audioConfig.SkillAudio);
                    }
                    
                });
            }
        }

        /// <summary>
        /// 播放击中音效
        /// </summary>
        public void PlayHitAudio()
        {
            AudioController.GetInstance().PlaySoundByAudioClip(_skillDataConfig.SkillConfig.SkillHitSound , false , 100);
        }
        
        
    }
}
