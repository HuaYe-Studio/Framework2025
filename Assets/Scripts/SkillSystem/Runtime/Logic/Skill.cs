using System;
using SkillSystem.Character;
using SkillSystem.Config;
using UnityEngine;

namespace SkillSystem.Runtime
{
    public partial class Skill
    {
        
        public int _skillID;
        
        
        private CharacterBattleBase _characterBattle;
        
        private SkillDataConfig _skillDataConfig;

        //释放技能后摇
        public Action<Skill> OnReleaseAfterCallBack;

        //释放技能结束回调
        public Action<Skill, bool> OnReleaseEnd;

        public SkillState CurrentSkillState; 
        

        
        
        //当前累计运行时间(ms)
        private int _accLogicTimeMS = 0;
        //当前逻辑帧
        private int _currentLogicTime=0;
        
        

        public Skill(int skillID, CharacterBattleBase characterBattle)
        {
            _skillID = skillID;
            _characterBattle = characterBattle;
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
            _characterBattle.PlayAnim(_skillDataConfig.SkillConfig.SkillId.ToString());
        }

        public void SkillStart()
        {

            _isTriggeredDamage = false;
            _isTriggeredOver = false;
            _hashEndCache.Clear();
            _hashTriggerCache.Clear();
            
            _effects.Clear();
            
            _accLogicTimeMS = 0;
            _currentLogicTime = 0;


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
            // if(CurrentSkillState == SkillState.None) return;
            // _accUpdateTimeMS += (int)(Time.deltaTime * 1000f);
            // if (CurrentSkillState == SkillState.Before &&
            //     _accUpdateTimeMS >= _skillDataConfig.SkillConfig.SkillShakeAfterTimeMS)
            // {
            //     SkillAfter();
            // }
            //
            // // OnUpdateDamage();
            // // OnUpdateEffect();
            // //容错率
            // if (_accUpdateTimeMS >= _skillDataConfig.CharacterConfig.AnimationClip.length * 1000f - 100)
            // {
            //     SkillEnd();
            // }
        }

        public void OnLogicFrameUpdate()
        {
            if(CurrentSkillState == SkillState.None) return;
            
            _accLogicTimeMS = _currentLogicTime * LogicFrameConfig.LogicFrameIntervalms;
            
            
            if (CurrentSkillState == SkillState.Before &&
                _accLogicTimeMS >= _skillDataConfig.SkillConfig.SkillShakeAfterTimeMS)
            {
                SkillAfter();
                
            }
            //更新不同配置的逻辑帧，处理逻辑
            
            //更新特效逻辑帧
            OnLogicFrameUpdateEffect();
            //更新伤害逻辑帧
            OnLogicFrameUpdateDamage();
            //更新行动逻辑帧  //本项目不使用定点数，直接使用根运动，因此毙掉
            // OnLogicFrameUpdateMove();
            //更新音效逻辑帧
            OnLogicFrameUpdateAudio();

            //更新子弹逻辑帧
            
            Debug.Log(_skillDataConfig.CharacterConfig.LogicFrame);

            if (_currentLogicTime == _skillDataConfig.CharacterConfig.MaxLogicFrame)
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