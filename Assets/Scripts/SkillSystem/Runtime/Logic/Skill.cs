using System;
using SkillSystem.Character;
using SkillSystem.Config;
using UnityEngine;

namespace SkillSystem.Runtime
{
    public partial class Skill
    {
        
        public int _skillID;
        
        
        private CharacterBase _character;
        
        private SkillDataConfig _skillDataConfig;

        //释放技能后摇
        public Action<Skill> OnReleaseAfterCallBack;

        //释放技能结束回调
        public Action<Skill, bool> OnReleaseEnd;

        public SkillState CurrentSkillState; 
        

        
        private int _accUpdateTimeMS = 0;
        
        

        public Skill(int skillID, CharacterBase character)
        {
            _skillID = skillID;
            _character = character;
            _skillDataConfig = Resources.Load<SkillDataConfig>("SkillData/" + _skillID);
        }

        public void ReleaseSkill(Action<Skill> releaseAfterCallBack , Action<Skill,bool> releaseEnd)
        {
            OnReleaseEnd = releaseEnd;
            OnReleaseAfterCallBack = releaseAfterCallBack;
            SkillStart();
            CurrentSkillState = SkillState.Before;
            PlayAnim();
            
            
        }


        public void PlayAnim()
        {
           
            
           
            _character.PlayAnim(_skillDataConfig.CharacterConfig.AnimationClip.name);
            
            
        }

        public void SkillStart()
        {

            _accUpdateTimeMS = 0;
            _isTriggeredDamage = false;
            _isTriggeredOver = false;
            _hashEndCache.Clear();
            _hashTriggerCache.Clear();
            
            _effects.Clear();
            _effectEnd.Clear();
            
            
        }

        public void SkillEnd()
        {
            CurrentSkillState = SkillState.End;
            OnReleaseEnd?.Invoke(this , false);
        }

        public void SkillAfter()
        {
            CurrentSkillState = SkillState.After;
            OnReleaseAfterCallBack?.Invoke(this);
            
            
            
        }

       

        public void OnUpdate()
        {
            if(CurrentSkillState == SkillState.None) return;
            _accUpdateTimeMS += (int)(Time.deltaTime * 1000f);
            if (CurrentSkillState == SkillState.Before &&
                _accUpdateTimeMS >= _skillDataConfig.SkillConfig.SkillShakeAfterTimeMS)
            {
                SkillAfter();
            }
            
            OnUpdateDamage();
            OnUpdateEffect();
            //容错率
            if (_accUpdateTimeMS >= _skillDataConfig.CharacterConfig.AnimationClip.length * 1000f - 100)
            {
                SkillEnd();
            }
        }
        
        public enum SkillState
        {
            None,
            Before,
            After,
            End
        }
        
        
        
        
        
        
        
        
    }
}