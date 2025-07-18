using FixMath;
using SkillSystem.Config;
using UnityEngine;

namespace LogicLayer
{
    public partial class LogicActor
    {

        private FixIntVector3 _moveDir;

        public void OnLoigcFrameUpdateMove()
        {
            if(ActionState != LogicObjectActionState.Idle && ActionState!= LogicObjectActionState.Move) return;
            
            //计算逻辑位置

            LogicPos +=  _moveDir * LogicMoveSpeed * LogicFrameConfig.LogicFrameInterval;

            //逻辑朝向
            if (LogicDir != _moveDir)
            {
                LogicDir = _moveDir;
            }
            
            //逻辑轴向
            if (LogicDir.x != FixInt.Zero)
            {
                LogicAxis = LogicDir.x > 0 ? 1 : -1;
            }



        }
        
        
        public void InputLogicFrameEvent(FixIntVector3 inputDir)
        {
            _moveDir = inputDir;
        }
        
        
        
    }
}
