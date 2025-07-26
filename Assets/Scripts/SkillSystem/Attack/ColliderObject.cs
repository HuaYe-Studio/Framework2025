using System;
using System.Collections.Generic;
using SkillSystem.Character;
using SkillSystem.Config;
using UnityEngine;

namespace SkillSystem.Attack
{
    /// <summary>
    /// 攻击检测器
    /// </summary>
    public class ColliderObject : MonoBehaviour
    {
        /// <summary>
        /// 是否触发
        /// </summary>
        private LogicObjectType _target;
        private SkillDamageConfig _skillDamageConfig;
        private Dictionary<int , CharacterBase> _charactersCache = new Dictionary<int , CharacterBase>();
        private CharacterBase _character;

        public bool Trigger = false;
       

        private void OnTriggerStay(Collider other)
        {
            if(!Trigger) return; 
            if (other.TryGetComponent(out CharacterBase character))
            {
                if(character.CharacterId == _character.CharacterId) return;
                //处理攻击
                Debug.Log(character.SelfType);
                Debug.Log(character.gameObject.name);
                if (character.SelfType == _target)
                {
                    if (_skillDamageConfig.TriggerIntervalMs == 0)
                    {

                        //处理一次性攻击
                        if (!_charactersCache.ContainsKey(character.CharacterId))
                        {
                            character.OnHit(_skillDamageConfig);
                            _charactersCache.Add(character.CharacterId, character);

                        }
                    }
                }
            }
            
        }

        
        public void Init(LogicObjectType target , SkillDamageConfig skillDamageConfig , CharacterBase character)
        {
            Debug.Log("开始检测");
            Trigger = true;
            _target = target;
            _character = character;
            _skillDamageConfig = skillDamageConfig;
        }

        public void OnAttackOver()
        {
            Debug.Log("清理缓存");
            _charactersCache.Clear();
            Trigger = false;
        }
        
        
    }
}