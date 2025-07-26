using System;
using System.Collections.Generic;
using SkillSystem.Attack;
using SkillSystem.Config;
using SkillSystem.Runtime;
using UnityEngine;

namespace SkillSystem.Character
{
    public class CharacterBase : MonoBehaviour
    {
        protected int _health;
        protected int _mana;
        protected LogicObjectType _selfType;
        
        protected int _characterId;
        
        public int Health { get { return _health; } set { _health = value; } }
        public int Mana { get { return _mana; } set { _mana = value; } }
        public int CharacterId { get { return _characterId; } set { _characterId = value; } }
        public LogicObjectType SelfType { get { return _selfType; } set { _selfType = value; } }
        
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;
        
        protected Animator _animator;
        
        private Runtime.SkillSystem _skillSystem;
        //普攻ID数组
        private int[] _normalSkillArr = new int[] {1001 , 1002 , 1003,};

        public List<Skill> ReleaseSkills = new List<Skill>();
        
        private int _currentNormalSkillIndex = 0;

        //拖取
        public ColliderObject _collider;

        public virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            InitSkill();
        }

        public virtual void OnHit(SkillDamageConfig damageConfig)
        {
            
        }

        public virtual void InitSkill()
        {
            _skillSystem = new SkillSystem.Runtime.SkillSystem(this);
            _skillSystem.InitSkills(_normalSkillArr);
        }

        public virtual void Update()
        {
            OnUpdateSkill();
        }

        public void OnUpdateSkill()
        {
            _skillSystem.OnUpdate();
        }
        
        public void OnReleaseSkill(int skillID)
        {
            Skill skill =  _skillSystem.ReleaseSkill(skillID,OnSkillReleaseAfter,OnSkillReleaseEnd);
            if (skill != null)
            {
                ReleaseSkills.Add(skill);
                if (!IsNormalSkillRelease(skillID))
                {
                    _currentNormalSkillIndex = 0;
                }
            }
        }
        
        public void OnSkillReleaseAfter(Skill skill)
        {
            if (IsNormalSkillRelease(skill._skillID))
            {
                // _currentNormalSkillIndex = _currentNormalSkillIndex == _normalSkillArr.Length - 1 ? 0 : _currentNormalSkillIndex + 1;
                _currentNormalSkillIndex++;
                if(_currentNormalSkillIndex >= _normalSkillArr.Length) _currentNormalSkillIndex = 0;
            }
            else
            {
                _currentNormalSkillIndex = 0;
            }
        }

        public void OnSkillReleaseEnd(Skill skill)
        {
            ReleaseSkills.Remove(skill);
            if (ReleaseSkills.Count == 0)
            {
                _currentNormalSkillIndex = 0;
            }
        }

        public void PlayAnim(string clipName)
        {
            _animator.CrossFadeInFixedTime(clipName , 0.155555f);
        }
        
        public void ReleaseNormalSkill()
        {
            OnReleaseSkill(_normalSkillArr[_currentNormalSkillIndex]);
        }
        
        public bool IsNormalSkillRelease(int skillID)
        {
            foreach (var VARIABLE in _normalSkillArr)
            {
                if(skillID == VARIABLE) return true;
            }
            return false;
        }
    }

    public enum LogicObjectType
    {
        Player,
        Enemy,
        Effect,
    }
}