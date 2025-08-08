using System;
using SkillSystem.Attack;
using SkillSystem.Character;
using SkillSystem.Config;
using UnityEngine;

namespace SkillSystem.Temp
{
    public class HuTaoLogic : CharacterBattleBase
    {
        
        public override void Awake()
        {
            base.Awake();
            _selfType = LogicObjectType.Player;
            CharacterId = 1000;
            _normalSkillArr = new[] { 1001, 1002, 1003, 1004 ,};
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
            if (InputManager.Instance.LAttack)
            {
                ReleaseNormalSkill();
            }
        }

        protected override void SetNormalSkill()
        {
            base.SetNormalSkill();
            _normalSkillArr = new[] { 1001, 1002, 1003, 1004, };
        }
    }
}