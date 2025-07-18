using System.Collections.Generic;
using SkillSystem.Config;
using SkillSystem.Runtime.Logic;
using SkillSystem.Runtime.Render;
using UnityEngine;

namespace SkillSystem.Runtime
{
    public partial class Skill
    {
        
        //Key 为 HashCode  Value 为特效对象
        private Dictionary<int , SkillEffectLogic> _effects = new Dictionary<int , SkillEffectLogic>();
        
        // ReSharper disable Unity.PerformanceAnalysis
        public void OnLogicFrameUpdateEffect()
        {
            if (_skillDataConfig.EffectConfigs != null && _skillDataConfig.EffectConfigs.Count > 0)
            {
                _skillDataConfig.EffectConfigs.ForEach(effect =>
                {
                    if (effect != null && _currentLogicTime == effect.TriggerFrame)
                    {
                        DestroyEffect(effect);

                        //生成特效
                        GameObject effectCache = GameObject.Instantiate(effect.Effect);
                        effectCache.transform.localPosition = Vector3.zero;
                        effectCache.transform.localScale = Vector3.one;
                        effectCache.transform.localRotation = Quaternion.identity;

                        SkillEffectRender skillEffectRender = effectCache.GetComponent<SkillEffectRender>();
                        if (skillEffectRender == null)
                        {
                            skillEffectRender = effectCache.AddComponent<SkillEffectRender>();
                        }
                        
                        
                        
                        
                        SkillEffectLogic effectLogic =
                            new SkillEffectLogic(LogicObjectType.Effect, effect, skillEffectRender, _skillCreator);
                        skillEffectRender.SetLogicObject(effectLogic);
                        _effects.Add(effect.GetHashCode(), effectLogic);
                        
                        
                    }

                    if (effect != null &&_currentLogicTime == effect.EndFrame)
                    {
                        //销毁特效
                        DestroyEffect(effect);
                        

                    }
                    
                    
                    
                });
            }
        }


        private void DestroyEffect(EffectConfig effectConfig)
        {
            
            if (_effects.TryGetValue(effectConfig.GetHashCode(), out SkillEffectLogic effectLogic))
            {
                _effects.Remove(effectConfig.GetHashCode());
                effectLogic.OnDestroy();
            }
        }
    }
}