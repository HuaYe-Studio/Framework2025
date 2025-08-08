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
        

        
        private int _accUpdateTimeMS = 0;
        
        //当前累计运行时间
        private int _accLogicTime=0;
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

            _accUpdateTimeMS = 0;
            _isTriggeredDamage = false;
            _isTriggeredOver = false;
            _hashEndCache.Clear();
            _hashTriggerCache.Clear();
            
            _effects.Clear();
            _effectEnd.Clear();
            
            _accLogicTime = 0;
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

        public void OnLogicFrameUpdate()
        {
            // if(CurrentSkillState == SkillState.None) return;
            //
            // _accLogicTime = _currentLogicTime * LogicFrameConfig.LogicFrameIntervalms;
            //
            // if (CurrentSkillState == SkillState.Before &&
            //     _accLogicTime >= _skillDataConfig.SkillConfig.SkillShakeAfterTimeMS)
            // {
            //     SkillAfter();
            //     
            // }
            //更新不同配置的逻辑帧，处理逻辑
            
            //更新特效逻辑帧
            // OnLogicFrameUpdateEffect();
            //更新伤害逻辑帧
            // OnLogicFrameUpdateDamage();
            //更新行动逻辑帧  //不适用定点数，因此毙掉
            // OnLogicFrameUpdateMove();
            //更新音效逻辑帧
            OnLogicFrameUpdateAudio();

            //更新子弹逻辑帧


            // if (_currentLogicTime == _skillDataConfig.CharacterConfig.LogicFrame)
            // {
            //     SkillEnd();
            // }
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