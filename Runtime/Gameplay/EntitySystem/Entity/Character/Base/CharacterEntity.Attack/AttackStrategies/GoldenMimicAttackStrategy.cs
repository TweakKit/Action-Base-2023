using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class GoldenMimicAttackStrategy : MeleeAttackStrategy
    {
        #region Members

        private readonly float _knockUpHeight = 4;
        private readonly float _knockUpVelocity = 12;
        protected StatusEffectModel[] damageStatusEffectModels;

        #endregion Members

        #region Class Methods

        public override void Init(CharacterModel ownerCharacterModel, Transform ownerCharacterTransform)
        {
            base.Init(ownerCharacterModel, ownerCharacterTransform);
            damageStatusEffectModels = new StatusEffectModel[1];
            KnockUpStatusEffectModel knockUpStatusEffectModel = new KnockUpStatusEffectModel(_knockUpHeight, _knockUpVelocity);
            damageStatusEffectModels[0] = knockUpStatusEffectModel;
        }

        protected override void DamageToTarget(CancellationToken cancellationToken)
        {
            if (IsTargetInAttackRange())
            {
                var damageInfo = ownerCharacterModel.GetDamageInfo(DamageSource.FromAttack, 1, damageStatusEffectModels, ownerCharacterModel.CurrentAttackedTarget.Model, causeNoDamage: true);
                var damageDirection = (ownerCharacterModel.CurrentAttackedTarget.Position - ownerCharacterModel.Position).normalized;
                ownerCharacterModel.CurrentAttackedTarget.GetHit(damageInfo, damageDirection);
            }
        }

        #endregion Class Methods
    }
}