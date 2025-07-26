namespace SkillSystem.Runtime
{
    public partial class Skill
    {



        /// <summary>
        /// 音效逻辑帧更新
        /// </summary>
       

        /// <summary>
        /// 播放击中音效
        /// </summary>
        public void PlayHitAudio()
        {
            AudioController.GetInstance().PlaySoundByAudioClip(_skillDataConfig.SkillConfig.SkillHitSound , false , 100);
        }
        
        
    }
}
