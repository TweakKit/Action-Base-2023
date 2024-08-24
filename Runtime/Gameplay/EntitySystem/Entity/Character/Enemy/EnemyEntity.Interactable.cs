using UnityEngine;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EnemyEntity : CharacterEntity<EnemyModel>
    {
        #region Properties

        public override bool IsMainHero
            => false;

        #endregion Properties

        #region Class Methods

        public override bool CanGetAffected(EntityModel interactingModel, DamageSource damageSource)
        {
            if (!ownerModel.IsDead)
                return interactingModel.EntityType.IsHero();
            else
                return false;
        }

        public override void GetHit(DamageInfo damageInfo, Vector2 damageDirection)
        {
            if (ownerModel.IsImmortal)
                ImmortalGetHit(damageInfo, damageDirection);
            else
                NormalGetHit(damageInfo, damageDirection);
        }

        private void NormalGetHit(DamageInfo damageInfo, Vector2 damageDirection)
            => base.GetHit(damageInfo, damageDirection); 

        private void ImmortalGetHit(DamageInfo damageInfo, Vector2 damageDirection)
        {
            if (ownerModel.CurrentHp - damageInfo.damage <= 0)
                return;

            if (ownerModel.CurrentHp / ownerModel.MaxHp <= Constant.ENEMY_IMMORTAL_HP_RATE)
                return;

            base.GetHit(damageInfo, damageDirection);
        }

        #endregion Class Methods
    }
}