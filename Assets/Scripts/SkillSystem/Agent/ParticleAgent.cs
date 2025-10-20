using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace SkillSystem.Agent
{
    public class ParticleAgent
    {
#if UNITY_EDITOR
    
        private double _lastRuntime;
        
        private ParticleSystem[] _particleSystems;
        
        public bool IsPlaying = false;
    
        public void InitPlayAnim(Transform trans)
        {
            _particleSystems = trans.GetComponentsInChildren<ParticleSystem>();
            EditorApplication.update += OnUpdate;
        }

        public void OnDestroy()
        {
            IsPlaying = false;
            EditorApplication.update -= OnUpdate;
        }
        
        

        public void OnUpdate()
        {
            if(!IsPlaying) return;
            
            if (_lastRuntime == 0)
            {
                _lastRuntime = EditorApplication.timeSinceStartup;
            }
            //当前运行时间
            double currentTime = EditorApplication.timeSinceStartup - _lastRuntime;
        
            Debug.Log("OnUpdate:" + currentTime);
            if (_particleSystems != null)
            {
                _particleSystems.ForEach(x =>
                {
                    if (x != null)
                    {
                        //停止所有粒子动效
                        x.Stop(true , ParticleSystemStopBehavior.StopEmittingAndClear);
                        //关闭随机种子
                        x.useAutoRandomSeed = false;
                        //模拟
                        x.Simulate((float)currentTime);
                        
                    }
                    
                    
                });
            }
        }

        public void OnSimulate(float frameCount)
        {
            if (_particleSystems != null)
            {
                _particleSystems.ForEach(x =>
                {
                    if (x != null)
                    {
                        //停止所有粒子动效
                        x.Stop(true , ParticleSystemStopBehavior.StopEmittingAndClear);
                        //关闭随机种子
                        x.useAutoRandomSeed = false;
                        //模拟
                        x.Simulate(frameCount);
                    }
                    
                    
                });
            }
        }
    
#endif
    }
}