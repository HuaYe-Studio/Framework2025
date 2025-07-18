using System;
using System.Collections.Generic;
using LogicLayer;
using Sirenix.Utilities;
using UnityEngine;

namespace SkillSystem.Runtime
{
    public class SkillSystem
    {
        
        private LogicActor _actor;
        
        private List<Skill> _skills = new List<Skill>();

        public SkillSystem(LogicActor actor)
        {
            _actor = actor;
        }

        public void InitSkills(int[] skillIds)
        {
            skillIds.ForEach(id =>
            {
                Skill skill = new Skill(id, _actor);
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

        public void OnLogicFrameUpdate()
        {
            _skills.ForEach(skill => skill.OnLogicFrameUpdate());
        }
    }
}