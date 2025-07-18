using System.Collections.Generic;
using UnityEngine;
using SkillSystem.Runtime;
namespace LogicLayer
{
    public partial class LogicActor
    {


        private SkillSystem.Runtime.SkillSystem _skillSystem;
        
        //普攻ID数组
        private int[] _normalSkillArr = new int[] {1001 , };

        public List<Skill> ReleaseSkills = new List<Skill>();

        public void InitActorSkill()
        {
            _skillSystem = new SkillSystem.Runtime.SkillSystem(this);
            _skillSystem.InitSkills(_normalSkillArr);

        }
        
        
        
        public void OnLogicFrameUpdateSkill()
        {
            _skillSystem.OnLogicFrameUpdate();
        }

        public void OnReleaseSkill(int skillID)
        {
            Skill skill =  _skillSystem.ReleaseSkill(skillID,OnSkillReleaseAfter,OnSkillReleaseEnd);
            if(skill != null) ReleaseSkills.Add(skill);
        }

        public void OnSkillReleaseAfter(Skill skill)
        {
            
        }

        public void OnSkillReleaseEnd(Skill skill)
        {
            ReleaseSkills.Remove(skill);
        }
        
        
    }
}
