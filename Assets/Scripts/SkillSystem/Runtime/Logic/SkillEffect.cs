using System.Collections.Generic;
using NUnit.Framework;
using SkillSystem.Config;
using UnityEngine;

namespace SkillSystem.Runtime
{
    public partial class Skill
    {
        
        //Key 为 HashCode  Value 为特效对象
        private Dictionary<int , SkillEffectConfig> _effects = new Dictionary<int , SkillEffectConfig>();
        
        // ReSharper disable Unity.PerformanceAnalysis
        
        private Dictionary<int , SkillEffectConfig> _effectEnd = new Dictionary<int , SkillEffectConfig>();
        
        private Dictionary<int , GameObject> _effectTargets = new Dictionary<int , GameObject>();
        
       

        public void OnUpdateEffect()
        {
            if (_skillDataConfig.EffectConfigs is { Count: > 0 })
            {
                _skillDataConfig.EffectConfigs.ForEach(effect =>
                {

                    if (effect != null && _accUpdateTimeMS >= effect.TriggerFrame &&  !_effects.ContainsKey(effect.GetHashCode()))
                    {
                        _effects.Add(effect.GetHashCode(), effect);
                        //生成特效
                        GameObject effectCache = GameObject.Instantiate(effect.Effect);
                        effectCache.transform.position = _characterBattle.transform.position + effect.EffectOffset;
                        effectCache.transform.localEulerAngles = _characterBattle.transform.eulerAngles + effect.EffectRotate;
                        _effectTargets.Add(effect.GetHashCode(), effectCache);
                        
                        
                    }

                    if (effect != null &&_accUpdateTimeMS >= effect.EndFrame &&  !_effectEnd.ContainsKey(effect.GetHashCode()))
                    {
                        //销毁特效
                        _effectEnd.Add(effect.GetHashCode(), effect);
                        GameObject.Destroy(_effectTargets[effect.GetHashCode()]);
                        _effectTargets.Remove(effect.GetHashCode());
                        

                    }
                });
            }
        }


        
    }
}