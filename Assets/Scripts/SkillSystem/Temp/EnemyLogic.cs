using SkillSystem.Character;
using SkillSystem.Config;
using UnityEngine;

namespace SkillSystem.Temp
{
    public class EnemyLogic : CharacterBase
    {
        public override void Awake()
        {
            base.Awake();
            _selfType = LogicObjectType.Enemy;
            CharacterId = EnmeyID;
        }

        public int EnmeyID;

        public override void OnHit(SkillDamageConfig damageConfig)
        {
            Debug.Log("Enemy hit");
            PlayAnim("GhostSamurai_APose_Large_Hit_2_Root");
        }
    }
}