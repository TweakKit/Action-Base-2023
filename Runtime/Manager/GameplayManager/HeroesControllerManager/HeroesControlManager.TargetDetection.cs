using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Definition;
using Runtime.Common.Singleton;
using Runtime.Gameplay.EntitySystem;
using Runtime.Manager.Data;

namespace Runtime.Gameplay.Manager
{
    public sealed partial class HeroesControlManager : MonoSingleton<HeroesControlManager>
    {
        #region Members

        [Header("--- TARGET DETECTION ---")]
        [SerializeField]
        private EntityTargetDetector _largeBoundCatchTargetDetector;
        [SerializeField]
        private EntityTargetDetector _smallBoundCatchTargetDetector;
        private List<IInteractable> _largeBoundDetectedTargets;
        private List<IInteractable> _smallBoundDetectedTargets;

        #endregion Members

        #region Class Methods

        public float BuffTargetEnemyDetectionRangeOnCamp(float valueDetectBuff)
        {
            var newRange = _heroesGroupFormationRadius + Constant.HERO_TEAM_DETECTED_RANGE + valueDetectBuff;
            _largeBoundCatchTargetDetector.UpdateDetectRange(newRange);
            _smallBoundCatchTargetDetector.UpdateDetectRange(newRange);
            return newRange;
        }

        public void ResetTargetEnemyDetectionRange()
        {
            UpdateTargetEnemyDetectionRange();
            UpdateTargetObjectDetectionRange();
        }

        public void RefindTargetForHero(HeroModel heroModel, List<IInteractable> ignoredTargets)
        {
            if (heroModel.CanUpdateTargetedTarget())
            {
                var heroClosestTarget = GetHeroClosestTargetExceptIgnored(heroModel, _smallBoundDetectedTargets, ignoredTargets);
                if (heroClosestTarget == null && heroModel.IsRangedAttack && _largeBoundDetectedTargets.Count > 0)
                    heroClosestTarget = GetHeroClosestTargetExceptIgnored(heroModel, _largeBoundDetectedTargets, ignoredTargets);
                heroModel.UpdateTargetedTarget(heroClosestTarget);
            }
        }

        private partial void InitTargetsDetector()
        {
            _largeBoundDetectedTargets = new List<IInteractable>();
            _smallBoundDetectedTargets = new List<IInteractable>();
            _largeBoundCatchTargetDetector.Init(0.0f, OnTargetEnteredLargeBound, OnTargetExitedLargeBound);
            _smallBoundCatchTargetDetector.Init(0.0f, OnTargetEnteredSmallBound, OnTargetExitedSmallBound);
        }

        private partial void UpdateTargetEnemyDetectionRange()
        {
            if (DataManager.Transitioned.IsOnCamping)
                return;
            _largeBoundCatchTargetDetector.UpdateDetectRange(_heroesGroupFormationRadius + Constant.HERO_TEAM_DETECTED_RANGE);
        }

        private partial void UpdateTargetObjectDetectionRange()
        {
            if (DataManager.Transitioned.IsOnCamping)
                return;
            _smallBoundCatchTargetDetector.UpdateDetectRange(_heroesGroupExploitRadius);
        }

        private void OnTargetEnteredLargeBound(IInteractable target)
        {
            if (!_largeBoundDetectedTargets.Contains(target))
                _largeBoundDetectedTargets.Add(target);

            foreach (var heroModelTransform in _heroModelTransforms)
            {
                var heroModel = heroModelTransform.Model;
                if (heroModel.IsRangedAttack && heroModel.CanUpdateTargetedTarget())
                {
                    var newHeroClosestTarget = GetHeroClosestTarget(heroModel, _largeBoundDetectedTargets);
                    if (newHeroClosestTarget != null)
                    {
                        if (heroModel.CurrentTargetedTarget == null)
                            heroModel.UpdateTargetedTarget(newHeroClosestTarget);
                        else if (newHeroClosestTarget.IsEnemyOrBoss && !heroModel.CurrentTargetedTarget.IsEnemyOrBoss)
                            heroModel.UpdateTargetedTarget(newHeroClosestTarget);
                    }
                }
            }
        }

        private void OnTargetExitedLargeBound(IInteractable target)
        {
            _largeBoundDetectedTargets.Remove(target);
            foreach (var heroModelTransform in _heroModelTransforms)
            {
                var heroModel = heroModelTransform.Model;
                if (heroModel.IsRangedAttack && heroModel.CanUpdateTargetedTarget())
                {
                    if (target == heroModel.CurrentTargetedTarget)
                    {
                        var heroClosestTarget = GetHeroClosestTarget(heroModel, _largeBoundDetectedTargets);
                        if (heroClosestTarget == null)
                            heroClosestTarget = GetHeroClosestTarget(heroModel, _smallBoundDetectedTargets);
                        heroModel.UpdateTargetedTarget(heroClosestTarget);
                    }
                }
            }
        }

        private void OnTargetEnteredSmallBound(IInteractable target)
        {
            if (!_smallBoundDetectedTargets.Contains(target))
                _smallBoundDetectedTargets.Add(target);

            foreach (var heroModelTransform in _heroModelTransforms)
            {
                var heroModel = heroModelTransform.Model;
                if (heroModel.CanUpdateTargetedTarget())
                {
                    var newHeroClosestTarget = GetHeroClosestTarget(heroModel, _smallBoundDetectedTargets);
                    if (newHeroClosestTarget != null)
                    {
                        if (heroModel.CurrentTargetedTarget == null)
                            heroModel.UpdateTargetedTarget(newHeroClosestTarget);
                        else if (newHeroClosestTarget.IsEnemyOrBoss && !heroModel.CurrentTargetedTarget.IsEnemyOrBoss)
                            heroModel.UpdateTargetedTarget(newHeroClosestTarget);
                    }
                }
            }
        }

        private void OnTargetExitedSmallBound(IInteractable target)
        {
            _smallBoundDetectedTargets.Remove(target);
            foreach (var heroModelTransform in _heroModelTransforms)
            {
                var heroModel = heroModelTransform.Model;
                if (heroModel.CanUpdateTargetedTarget())
                {
                    if (target == heroModel.CurrentTargetedTarget)
                    {
                        var heroClosestTarget = GetHeroClosestTarget(heroModel, _smallBoundDetectedTargets);
                        heroModel.UpdateTargetedTarget(heroClosestTarget);
                    }
                }
            }
        }

        private void UpdateAllHeroesTargets()
        {
            foreach (var heroModelTransform in _heroModelTransforms)
            {
                var heroModel = heroModelTransform.Model;
                if (heroModel.CanUpdateTargetedTarget())
                {
                    var heroClosestTarget = GetHeroClosestTarget(heroModel, _smallBoundDetectedTargets);
                    if (heroClosestTarget == null && heroModel.IsRangedAttack && _largeBoundDetectedTargets.Count > 0)
                        heroClosestTarget = GetHeroClosestTarget(heroModel, _largeBoundDetectedTargets);
                    if (heroModel.CurrentTargetedTarget != heroClosestTarget)
                        heroModel.UpdateTargetedTarget(heroClosestTarget);
                }
            }
        }

        private void CheckUpdateHeroesMissingTarget()
        {
            foreach (var heroModelTransform in _heroModelTransforms)
            {
                var heroModel = heroModelTransform.Model;
                if (heroModel.CurrentTargetedTarget == null)
                {
                    var heroClosestTarget = GetHeroClosestTarget(heroModel, _smallBoundDetectedTargets);
                    if (heroClosestTarget == null && heroModel.IsRangedAttack && _largeBoundDetectedTargets.Count > 0)
                        heroClosestTarget = GetHeroClosestTarget(heroModel, _largeBoundDetectedTargets);
                    heroModel.UpdateTargetedTarget(heroClosestTarget);
                }
            }
        }

        private IInteractable GetHeroClosestTarget(HeroModel heroModel, List<IInteractable> detectedTargets)
        {
            int highestPriority = int.MinValue;
            foreach (var detectedTarget in detectedTargets)
            {
                if (highestPriority < detectedTarget.DetectedPriority)
                    highestPriority = detectedTarget.DetectedPriority;
            }

            float closestSqrDistance = float.MaxValue;
            IInteractable closestTarget = null;

            foreach (var detectedTarget in detectedTargets)
            {
                if (detectedTarget.DetectedPriority == highestPriority)
                {
                    var sqrDistanceBetween = (heroModel.Position - detectedTarget.Position).sqrMagnitude;
                    if (closestSqrDistance > sqrDistanceBetween)
                    {
                        closestSqrDistance = sqrDistanceBetween;
                        closestTarget = detectedTarget;
                    }
                }
            }

            return closestTarget;
        }

        private IInteractable GetHeroClosestTargetExceptIgnored(HeroModel heroModel, List<IInteractable> detectedTargets, List<IInteractable> ignoredTargets)
        {
            int highestPriority = int.MinValue;
            foreach (var detectedTarget in detectedTargets)
            {
                if ((ignoredTargets == null || ignoredTargets.All(x => x != detectedTarget)) && highestPriority < detectedTarget.DetectedPriority)
                    highestPriority = detectedTarget.DetectedPriority;
            }

            float closestSqrDistance = float.MaxValue;
            IInteractable closestTarget = null;

            foreach (var detectedTarget in detectedTargets)
            {
                if ((ignoredTargets == null || ignoredTargets.All(x => x != detectedTarget)) && detectedTarget.DetectedPriority == highestPriority)
                {
                    var sqrDistanceBetween = (heroModel.Position - detectedTarget.Position).sqrMagnitude;
                    if (closestSqrDistance > sqrDistanceBetween)
                    {
                        closestSqrDistance = sqrDistanceBetween;
                        closestTarget = detectedTarget;
                    }
                }
            }

            return closestTarget;
        }

        public Vector2 GetCampingPosition()
            => HeroesGroupCenterHolder.position;

        #endregion Class Methods
    }
}