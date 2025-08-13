using System.Collections.Generic;
using System.Linq;
using SkillSystem.Character;
using SkillSystem.ColliderSystem;
using SkillSystem.Config;
using UnityEngine;

namespace SkillSystem.Runtime
{
    public partial class Skill
    {
        
        //当前伤害累加时间
        private int _accDamageTime = 0;

        private List<int> _targerCache  = new List<int>();

        public void OnLogicFrameUpdateDamage()
        {
            if (_skillDataConfig.DamageConfigs is { Count: > 0 })
            {
                _skillDataConfig.DamageConfigs.ForEach(damage =>
                {
                    if (_currentLogicTime >= damage.TriggerLogicFrame && _currentLogicTime < damage.EndLogicFrame)
                    {
                        
                        
                        
                        //一次性触发伤害，但在触发区间内一直检测，但只触发一次，需缓存目标
                        if (damage.TriggerIntervalMs == 0)
                        {
                            TriggerDamage(damage , _characterBattle);    
                        }

                        if (damage.TriggerIntervalMs > 0)
                        {
                            _accDamageTime += LogicFrameConfig.LogicFrameIntervalms;
                            if (_accDamageTime >= damage.TriggerIntervalMs)
                            {
                                //触发一次间隔伤害
                                TriggerDamage(damage , _characterBattle, true);
                                
                                _accDamageTime = 0;
                            }
                            
                        }
                        
                    }

                    if (_currentLogicTime == damage.EndLogicFrame)
                    {
                        OnRelease();
                    }
                });
            }
        }
        

        private IEnumerable<CharacterBattleBase> DamageDetection(SkillDamageConfig damageConfig , CharacterBattleBase characterBattle)
        {
            
            
            //进行范围检测
            Collider[] colliders = Physics.OverlapBox(_characterBattle.transform.position +
                                                      _characterBattle.transform.forward * damageConfig.BoxOffset.z +
                                                      _characterBattle.transform.right * damageConfig.BoxOffset.x +
                                                      _characterBattle.transform.up * damageConfig.BoxOffset.y,
                damageConfig.BoxSize / 2f, Quaternion.Euler(_characterBattle.transform.localEulerAngles));
            
            
            

            LogicObjectType targetType = LogicObjectType.None;
            switch (characterBattle.SelfType)
            {
                case LogicObjectType.Player:
                    switch (damageConfig.TargetType)
                    {
                        case TargetType.Enemy:
                            targetType = LogicObjectType.Enemy;
                            break;
                    }
                    break;
            }

            return colliders
                .Where(collider => collider != null)
                .Select(collider => collider.GetComponent<CharacterBattleBase>())
                .Where(target => target != null)
                .Where(target => target.SelfType == targetType).Distinct();
        }

        private void TriggerDamage(SkillDamageConfig damageConfig , CharacterBattleBase characterBattle , bool isAccDamage = false)
        {
            IEnumerable<CharacterBattleBase> targets = DamageDetection(damageConfig , characterBattle);
            //处理检测结果
            foreach (CharacterBattleBase target in targets)
            {
                
                
                void OnTriggerHandler()
                {
                    Debug.Log(target.name + "受到伤害");
                    target.OnHit(damageConfig);
                    
                    //TODO:添加Buff ，特效 ，音效
                    PlayHitAudio();
                    AddHitEffect(target);
                    
                }
                
                
                
                if (!isAccDamage)
                {
                    //一次性伤害 ，直接缓存
                    if (!_targerCache.Contains(target.GetHashCode()))
                    {
                        OnTriggerHandler();
                        _targerCache.Add(target.GetHashCode());
                    }
                }
                else
                {
                    OnTriggerHandler();
                }
                
               
            }
            
        }

        private void AddHitEffect(CharacterBattleBase characterBattle)
        {
            if (_skillDataConfig.SkillConfig.SkillHitEffect != null)
            {
                characterBattle.OnHitAddEffect(_skillDataConfig.SkillConfig.SkillHitEffect,_skillDataConfig.SkillConfig.SkillHitEffectDuration);
            }
        }

        

        public void OnRelease()
        {
            
        }
        
        
    }
}
