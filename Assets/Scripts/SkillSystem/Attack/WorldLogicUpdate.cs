using System;
using SkillSystem.Config;
using UnityEngine;

namespace SkillSystem.Attack
{
    public class WorldLogicUpdate : MonoSingleton<WorldLogicUpdate>
    {
        
        public Action OnLogicUpdate;
        
        

        private float _accLogicRunTime;
        private float _nextLogicFrameTime;
        public float LogicDeltaTime;
        protected override void Init()
        {
            Reset();
            //避免穿帧，提高帧率
            Application.targetFrameRate = 90;
        }

        public void Reset()
        {
            _accLogicRunTime = 0;
            _nextLogicFrameTime = 0;
            LogicDeltaTime = 0;
        }
        
        private void Update()
        {
            //逻辑帧的累计时间
            _accLogicRunTime += Time.deltaTime;
            
            //保证所有设备的逻辑帧率一致，并进行追帧操作
            while (_accLogicRunTime > _nextLogicFrameTime)
            {
                //更新逻辑帧
                OnLogicUpdate?.Invoke();
                //计算下一个逻辑帧运行的时间
                _nextLogicFrameTime += LogicFrameConfig.LogicFrameInterval;
                //逻辑帧ID进行自增
                LogicFrameConfig.LogicFrameid++;
            }
            
            //逻辑帧 1秒15帧 渲染帧 1秒60帧
            
            

            LogicDeltaTime = (_accLogicRunTime + LogicFrameConfig.LogicFrameInterval - _nextLogicFrameTime) / LogicFrameConfig.LogicFrameInterval;
        }
    }
}