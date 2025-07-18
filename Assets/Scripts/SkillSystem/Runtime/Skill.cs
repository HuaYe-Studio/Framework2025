using System;
using LogicLayer;
using SkillSystem.Config;
using UnityEngine;

namespace SkillSystem.Runtime
{
    public partial class Skill
    {
        
        public int _skillID;
        
        private LogicActor _skillCreator;
        
        private SkillDataConfig _skillDataConfig;

        //释放技能后摇
        public Action<Skill> OnReleaseAfterCallBack;

        //释放技能结束回调
        public Action<Skill, bool> OnReleaseEnd;

        public SkillState CurrentSkillState; 
        
        //当前累计运行时间
        private int _accLogicTime=0;
        //当前逻辑帧
        private int _currentLogicTime=0;
        
        

        public Skill(int skillID, LogicActor skillCreator)
        {
            _skillID = skillID;
            _skillCreator = skillCreator;
            _skillDataConfig = Resources.Load<SkillDataConfig>("SkillData/" + _skillID);
            Debug.Log("SkillDataConfig:" + _skillDataConfig.SkillConfig.SkillId);
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
            _skillCreator.PlayAnim(_skillDataConfig.CharacterConfig.AnimationClip.name);
            
        }

        public void SkillStart()
        {
            _currentLogicTime = 0;
            _accLogicTime = 0;
            
            
        }

        public void SkillEnd()
        {
            CurrentSkillState = SkillState.End;
            OnReleaseEnd?.Invoke(this , false);
        }

        public void SkillAfter()
        {
            CurrentSkillState = SkillState.After;
            
            
            
            
        }

        public void OnLogicFrameUpdate()
        {
            
            if(CurrentSkillState == SkillState.None) return;
            
            _accLogicTime = _currentLogicTime * LogicFrameConfig.LogicFrameIntervalms;

            if (CurrentSkillState == SkillState.Before &&
                _currentLogicTime >= _skillDataConfig.SkillConfig.SkillShakeAfterTimeMS)
            {
                SkillAfter();
                
            }
            //更新不同配置的逻辑帧，处理逻辑
            
            //更新特效逻辑帧
            OnLogicFrameUpdateEffect();
            //更新伤害逻辑帧
            
            //更新行动逻辑帧
            
            //更新音效逻辑帧
            
            //更新子弹逻辑帧


            if (_currentLogicTime == _skillDataConfig.CharacterConfig.LogicFrame)
            {
                SkillEnd();
            }
            //逻辑帧自增
            _currentLogicTime++;
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