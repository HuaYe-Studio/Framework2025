using System;
using SkillSystem.Character;
using SkillSystem.Config;
using UnityEngine;

namespace SkillSystem.Temp
{
    public class HuTaoLogic : CharacterBase
    {
        
        public override void Awake()
        {
            base.Awake();
            _selfType = LogicObjectType.Player;
            CharacterId = 1000;
        }

        public override void OnHit(SkillDamageConfig damageConfig)
        {
            //TODO：处理受伤逻辑
            Debug.Log("受到伤害");
        }

        public override void Update()
        {
            base.Update();
            PlayerInputHandler();
        }


        private void PlayerInputHandler()
        {
            if (InputManager.MainInstance.LAttack)
            {
                ReleaseNormalSkill();
            }
        }
    }
}