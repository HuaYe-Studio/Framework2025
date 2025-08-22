using SkillSystem.Character;
using SkillSystem.Config;
using UIFramework.Core;
using UIFramework.ViewPath;
using UnityEngine;

namespace SkillSystem.Temp
{
    public class EnemyLogic : CharacterBattleBase
    {
        public override int Health { get =>_health;
            set
            {
                _health = value;
                UIManager.Instance.GetPanel<MainPanelView>().GetComponent<MainPanelPresenter>().UpdateHP(MaxHealth , _health);
            } }

        public override void Awake()
        {
            base.Awake();
            _selfType = LogicObjectType.Enemy;
            CharacterId = EnmeyID;
            MaxHealth = 1000;
            Health = 1000;

        }

        public int EnmeyID;

        public override void OnHit(SkillDamageConfig damageConfig)
        {
            if(IsDead)
                return;
            
            Debug.Log("Enemy hit");
            PlayAnim("GhostSamurai_APose_Large_Hit_2_Root");

            Health -= damageConfig.DamageValue;
            Debug.Log($"伤害值：{damageConfig.DamageValue}，剩余血量：{Health}");
            if (Health <= 0)
            {
                IsDead = true;
                PlayAnim("Die");
            }
            


        }

        public override void OnHitAddEffect(GameObject effect, float duration)
        {
            if(IsDead)
                return;
            base.OnHitAddEffect(effect, duration);
        }
    }
}