using UnityEditor;
using UnityEngine;

namespace SkillSystem.Agent
{
    public class AnimationAgent
    {

#if UNITY_EDITOR
    
        private Animation _animation;
        private double _lastRuntime;

        public bool IsPlaying = false;
    
        public void InitPlayAnim(Transform trans)
        {
            _animation = trans.GetComponentInChildren<Animation>();
            EditorApplication.update += Onupdate;
        }

        public void OnDestroy()
        {
            EditorApplication.update -= Onupdate;
        }

        public void Onupdate()
        {
            if (!IsPlaying) return;
            
            if (_animation == null || _animation.clip == null) return;
            
            if (_lastRuntime == 0)
            {
                _lastRuntime = EditorApplication.timeSinceStartup;
            }
            //当前运行时间
            double currentTime = EditorApplication.timeSinceStartup - _lastRuntime;

            //动画播放进度
            float curAniNormalizationValue = (float)currentTime / _animation.clip.length;
        
            //动画采样
            _animation.clip.SampleAnimation(_animation.gameObject , (float)currentTime);
        }

        public void OnSimulate(float deltaTime)
        {
            if (_animation == null || _animation.clip == null) return;
            
            //动画播放进度
            _animation.clip.SampleAnimation(_animation.gameObject , deltaTime);
        }
    
#endif
    
    }
}
