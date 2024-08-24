using System;
using UnityEngine;
using Runtime.Gameplay.Manager;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public class KnockUpStatusEffect : PerFrameDurationStatusEffect<KnockUpStatusEffectModel>
    {
        #region Members

        private float _knockUpDuration;
        private float _currentKnockUpDuration;
        private float _knockUpHeight;
        private Vector2 _knockUpDirection;
        private Vector2 _originPosition;
        private bool _hasToStopKnockUp;

        #endregion Members

        #region Properties

        protected override float Duration => _knockUpDuration;

        #endregion Properties

        #region Class methods

        public override void Init(StatusEffectModel statusEffectModel, EntityModel senderModel, CharacterModel receiverModel, Vector2 statusEffectDirection)
        {
            base.Init(statusEffectModel, senderModel, receiverModel, statusEffectDirection);
            _currentKnockUpDuration = 0.0f;
            _originPosition = receiverModel.Position;
            _knockUpDirection = statusEffectDirection;
        }

        protected override void InitData(EntityModel senderModel)
        {
            base.InitData(senderModel);
            _knockUpDuration = ownerModel.KnockUpHeight / ownerModel.KnockUpVelocity;
            _knockUpHeight = ownerModel.KnockUpHeight;
        }

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            receiverModel.StartGettingKnockUp();
        }

        protected override void AffectPerFrame(CharacterModel receiverModel)
        {
            base.AffectPerFrame(receiverModel);
            _currentKnockUpDuration += Time.deltaTime;
            float interpolationValue = Mathf.Lerp(0, _knockUpHeight, Mathf.Clamp01(_currentKnockUpDuration / _knockUpDuration));
            Vector2 knockUpPosition = _originPosition + _knockUpDirection * interpolationValue;
            if (CanBeKnockedUpAtThisPosition(knockUpPosition))
                receiverModel.GettingKnockUp(knockUpPosition);
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            receiverModel.StopGettingKnockUp();
        }

        protected override bool CheckStopAffect(CharacterModel receiverModel)
            => base.CheckStopAffect(receiverModel) || _hasToStopKnockUp;

        private bool CanBeKnockedUpAtThisPosition(Vector2 position)
        {
            bool isWalkable = NavigationManager.Instance.IsWalkable(position);
            _hasToStopKnockUp = !isWalkable;
            return isWalkable;
        }

        #endregion Class methods
    }
}