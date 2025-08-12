using System.Collections.Generic;
using SkillSystem.Character;
using SkillSystem.Config;

namespace SkillSystem.Runtime
{
    public partial class Skill
    {

        //Key 为 HashCode  Value 为特效对象

        //当前伤害累加时间
        private int _accDamageTime = 0;
        private bool _isTriggeredDamage = false;
        private bool _isTriggeredOver = false;
        private List<int> _hashTriggerCache = new List<int>();
        private List<int> _hashEndCache = new List<int>();



        /// <summary>
        /// 逻辑帧更新伤害
        /// </summary>
        // public void OnLogicFrameUpdateDamage()
        // {
        //     //判断当前伤害配置列表是否为空
        //     if (_skillDataConfig.DamageConfigs != null && _skillDataConfig.DamageConfigs.Count > 0)
        //     {
        //         _skillDataConfig.DamageConfigs.ForEach(damage =>
        //         {
        //             int hashcode = damage.GetHashCode();
        //             if (_currentLogicTime == damage.TriggerLogicFrame)
        //             {
        //                 DestroyCollisionBody(damage);
        //                 //达到伤害触发帧时，生成碰撞体
        //                 ColliderBehaviour collider = CreateCollisionBody(damage);
        //                 //字典缓存
        //                 _collider.Add(hashcode, collider);
        //                 
        //                 //判断是否是一次性触发
        //                 if (damage.TriggerIntervalMs == 0)
        //                 {
        //                     //一次性触发，直接处理伤害
        //                     if (_collider.ContainsKey(hashcode))
        //                     {
        //                         TriggerDamage(_collider[hashcode] , damage);
        //                     }
        //                 }
        //                 
        //             }
        //             
        //             //处理伤害检测
        //             if (damage.TriggerIntervalMs > 0)
        //             {
        //                 //说明不是一次性触发，而是间隔触发
        //                 _accDamageTime += LogicFrameConfig.LogicFrameIntervalms;
        //                 
        //                 //如果累加时间大于帧间隔，则触发一次伤害
        //                 if (_accDamageTime >= damage.TriggerIntervalMs)
        //                 {
        //                     //TODO: 触发伤害
        //                     if (_collider.ContainsKey(hashcode))
        //                     {
        //                         TriggerDamage(_collider[hashcode] , damage);
        //                     }
        //                     
        //                     
        //                     _accDamageTime = 0;
        //                 }
        //             }
        //             
        //
        //
        //             if (_currentLogicTime == damage.EndLogicFrame)
        //             {
        //                 //达到结束帧时，销毁碰撞体
        //                 DestroyCollisionBody(damage);
        //             }
        //             
        //             
        //             
        //         });




        //     }
        // }

        public void OnLogicFrameUpdateDamage()
        {
            if (_skillDataConfig.DamageConfigs is { Count: > 0 })
            {
                _skillDataConfig.DamageConfigs.ForEach(damage =>
                {
                    if (_currentLogicTime == damage.TriggerLogicFrame)
                    {
                        switch (_characterBattle.SelfType)
                        {
                            case LogicObjectType.Player:
                                switch (damage.TargetType)
                                {
                                    case TargetType.Enemy:
                                        _characterBattle._collider.Init(LogicObjectType.Enemy , damage , _characterBattle);
                                        break;
                                }
                                break;
                        }
                    }

                    if (_currentLogicTime == damage.EndLogicFrame)
                    {
                        _characterBattle._collider.OnAttackOver();
                    }
                });
            }
        }

        // public void OnUpdateDamage()
        // {
        //     if (_skillDataConfig.DamageConfigs is { Count: > 0 })
        //     {
        //         _skillDataConfig.DamageConfigs.ForEach(damage =>
        //         {
        //             if (_accUpdateTimeMS >= damage.TriggerLogicFrame && !_hashTriggerCache.Contains(damage.GetHashCode()))
        //             {
        //                 _hashTriggerCache.Add(damage.GetHashCode());
        //                 switch (_characterBattle.SelfType)
        //                 {
        //                     case LogicObjectType.Player:
        //                         switch (damage.TargetType)
        //                         {
        //                             case TargetType.Enemy:
        //                                 _characterBattle._collider.Init(LogicObjectType.Enemy , damage , _characterBattle);
        //                                 break;
        //                         }
        //                         break;
        //                 }
        //             }
        //
        //             if (_accUpdateTimeMS >= damage.EndLogicFrame && !_hashEndCache.Contains(damage.GetHashCode()))
        //             {
        //                 _hashEndCache.Add(damage.GetHashCode());
        //                 _characterBattle._collider.OnAttackOver();
        //             }
        //         });
        //     }
        // }

        
        
    }
}
