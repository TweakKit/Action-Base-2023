using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class MeleeAOEAttackStrategy : MeleeAttackStrategy
    {
        #region Members

        private static readonly float s_rangeAOE = 1.5f;

        #endregion Members

        #region Class Methods

        protected override void DamageToTarget(CancellationToken cancellationToken)
        {
            if (IsTargetInAttackRange())
            {
                var colliders = Physics2D.OverlapCircleAll(ownerCharacterModel.CurrentAttackedTarget.Position, s_rangeAOE);
                foreach (var collider in colliders)
                {
                    var interactable = collider.GetComponent<IInteractable>();
                    if (interactable != null && interactable.CanGetAffected(ownerCharacterModel, DamageSource.FromAttack))
                    {
                        var damageInfo = GetDamageInfo(interactable.Model);
                        var damageDirection = (ownerCharacterModel.CurrentAttackedTarget.Position - interactable.Position).normalized;
                        interactable.GetHit(damageInfo, damageDirection);
                    }
                }
            }
        }

        #endregion Class Methods
    }
}