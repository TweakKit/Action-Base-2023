using System;
using Runtime.Gameplay.Manager;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public class PullToCenterEffect : PerFrameDurationStatusEffect<PullToCenterEffectModel>
    {
        private Vector2 _newPosition;
        private Vector2 _originPosition;

        private float _duration;
        private float _currentDuration;
        private float _pullSpeed;
        private bool _canPull;

        protected override float Duration => GetDuration();

        public override void Init(
              StatusEffectModel statusEffectModel
            , EntityModel senderModel
            , CharacterModel receiverModel
            , Vector2 statusEffectDirection
        )
        {
            if (statusEffectModel is PullToCenterEffectModel pullToCenterEffectModel)
            {
                _pullSpeed = pullToCenterEffectModel.GetPullSpeed();
                _duration = pullToCenterEffectModel.GetDuration();
            }

            _newPosition = receiverModel.Position - statusEffectDirection;
            _originPosition = receiverModel.Position;

            base.Init(statusEffectModel, senderModel, receiverModel, statusEffectDirection);
        }

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            receiverModel.StartGettingPullToCenter();
        }

        protected override void AffectPerFrame(CharacterModel receiverModel)
        {
            base.AffectPerFrame(receiverModel);

            _currentDuration += Time.deltaTime;

            if (_currentDuration >= _duration)
            {
                return;
            }

            // để t max là 0.9f vì không muốn enemy dính lai nhau khi pull
            var currentPosition = Vector2.Lerp(
                  _originPosition
                , _newPosition
                , Mathf.Min(0.9f, _currentDuration * _pullSpeed)
            );

            if (CanPull(currentPosition))
            {
                receiverModel.GettingPullToCenter(currentPosition);
            }
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            receiverModel.StopGettingPullToCenter();
        }

        protected override bool CheckStopAffect(CharacterModel receiverModel)
            => base.CheckStopAffect(receiverModel) || _canPull;

        private bool CanPull(Vector2 position)
        {
            bool isWalkable = NavigationManager.Instance.IsWalkable(position);
            _canPull = !isWalkable;
            return isWalkable;
        }

        private float GetDuration()
        {
            return _duration;
        }
    }
}