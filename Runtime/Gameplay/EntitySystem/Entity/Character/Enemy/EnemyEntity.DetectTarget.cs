using System.Collections.Generic;
using UnityEngine;
using Runtime.Gameplay.Manager;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EnemyEntity : CharacterEntity<EnemyModel>
    {
        #region Members

        protected List<HeroModelTransform> detectedTargets;

        #endregion Members

        #region Class Methods

        private partial void PartialValidateDetectTarget() { }
        private partial void PartialDisposeDetectTarget() { }

        private partial void PartialInitializeDetectTarget()
            => detectedTargets = EntitiesManager.Instance.GetTargetHeroModelTransforms(true);

        protected virtual void UpdateClosestTarget()
        {
            if (ownerModel.CanUpdateTargetedTarget())
            {
                var closestTarget = GetClosestTarget();
                if (closestTarget != null)
                {
                    if (ownerModel.CurrentTargetedTarget != closestTarget)
                        ownerModel.UpdateTargetedTarget(closestTarget);
                }
                else ownerModel.UpdateTargetedTarget(null);
            }
        }

        protected virtual IInteractable GetClosestTarget()
        {
            float closestSqrDistance = float.MaxValue;
            Transform closestTargetTransform = null;

            foreach (var detectedTarget in detectedTargets)
            {
                var sqrDistanceBetween = (ownerModel.Position - detectedTarget.Model.Position).sqrMagnitude;
                if (closestSqrDistance > sqrDistanceBetween)
                {
                    closestSqrDistance = sqrDistanceBetween;
                    closestTargetTransform = detectedTarget.Transform;
                }
            }

            if (closestTargetTransform != null)
                return closestTargetTransform.GetComponent<IInteractable>();
            else
                return null;
        }

        #endregion Class Methods
    }
}