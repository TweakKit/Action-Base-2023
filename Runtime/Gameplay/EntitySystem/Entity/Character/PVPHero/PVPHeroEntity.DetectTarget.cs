using System.Collections.Generic;
using UnityEngine;
using Runtime.Gameplay.Manager;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class PVPHeroEntity : HeroEntity
    {
        #region Members

        private List<HeroModelTransform> _detectedTargets;

        #endregion Members

        #region Class Methods

        private partial void PartialValidateDetectTarget() { }
        private partial void PartialDisposeDetectTarget() { }

        private partial void PartialInitializeDetectTarget()
            => _detectedTargets = EntitiesManager.Instance.GetTargetHeroModelTransforms(ownerModel.IsHeroBoss);

        private void UpdateClosestTarget()
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

        private IInteractable GetClosestTarget()
        {
            float closestSqrDistance = float.MaxValue;
            Transform closestTargetTransform = null;

            foreach (var detectedTarget in _detectedTargets)
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