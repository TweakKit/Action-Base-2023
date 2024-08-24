using UnityEngine;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class HeroEntity : CharacterEntity<HeroModel>
    {
        #region Members

        private float _currentIdleDuration;
        private float _idleDurationDelay;

        #endregion Members

        #region Class Methods

        private partial void PartialValidateStandStill() { }
        private partial void PartialDisposeStandStill() { }

        private partial void PartialInitializeStandStill()
        {
            _idleDurationDelay = Random.Range(Constant.CHARACTER_IDLE_MIN_DURATION, Constant.CHARACTER_IDLE_MAX_DURATION);
            _currentIdleDuration = 0.0f;
            ownerModel.ReactionChangedEvent += OnReactionChangedOnStandStill;
        }

        private partial void PartialUpdateStandStill()
        {
            _currentIdleDuration += Time.deltaTime;
            if (_currentIdleDuration >= _idleDurationDelay)
            {
                _currentIdleDuration = 0.0f;
                _idleDurationDelay = Random.Range(Constant.CHARACTER_IDLE_MIN_DURATION, Constant.CHARACTER_IDLE_MAX_DURATION);
                ownerModel.UpdateState(CharacterState.Move);
            }
        }

        private void OnReactionChangedOnStandStill(CharacterReactionType characterReactionType)
        {
            switch (characterReactionType)
            {
                case CharacterReactionType.JustStandStill:
                    _idleDurationDelay = Random.Range(Constant.CHARACTER_IDLE_MIN_DURATION, Constant.CHARACTER_IDLE_MAX_DURATION);
                    _currentIdleDuration = 0.0f;
                    ownerModel.MoveDirection = Vector2.zero;
                    break;
            }
        }

        #endregion Class Methods
    }
}