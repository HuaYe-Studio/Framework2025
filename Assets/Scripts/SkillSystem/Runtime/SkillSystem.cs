using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using SkillSystem.Character;
using UnityEngine;

namespace SkillSystem.Runtime
{
    public class SkillSystem
    {
        
        
        private CharacterBase _character;
        
        private List<Skill> _skills = new List<Skill>();

        public SkillSystem(CharacterBase character)
        {
            _character = character;
        }

        public void InitSkills(int[] skillIds)
        {
            skillIds.ForEach(id =>
            {
                Skill skill = new Skill(id, _character);
                _skills.Add(skill);

            });
            Debug.Log("技能初始化完成，技能个数:" + skillIds.Length);
        }

        public Skill ReleaseSkill(int skillID, Action<Skill> releaseAfterCallback, Action<Skill> releaseSkillEnd)
        {

            foreach (var skill in _skills)
            {
                if (skill._skillID == skillID)
                {
                    if (skill.CurrentSkillState == Skill.SkillState.Before
                        )
                    {
                        return null;
                    }
                    
                    skill.ReleaseSkill(releaseAfterCallback, (s, combinationSkill) =>
                    {
                        releaseSkillEnd?.Invoke(s);
                        
                        //若当前技能有组合技
                        if (combinationSkill)
                        {
                            
                        }
                    });
                    return skill;
                }
            }
            Debug.Log("技能不存在:" + skillID);
            return null;
        }

        // public void OnLogicFrameUpdate()
        // {
        //     _skills.ForEach(skill => skill.OnLogicFrameUpdate());
        // }

        public void OnUpdate()
        {
            _skills.ForEach(skill => skill.OnUpdate());
        }
    }
}