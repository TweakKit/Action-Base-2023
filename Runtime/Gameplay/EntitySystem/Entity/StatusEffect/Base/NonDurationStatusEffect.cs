using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    /// <summary>
    /// Non-Duration status effect affects the target in an unknown duration until a stop condition is met.
    /// Note: This doesn't do any extra task in every frame or/add every interval.
    /// </summary>
    public abstract class NonDurationStatusEffect<T> : OneShotStatusEffect<T> where T : StatusEffectModel
    {
        #region Class Methods

        public override void Init(StatusEffectModel statusEffectModel, EntityModel senderModel, CharacterModel receiverModel, Vector2 statusEffectDirection)
        {
            base.Init(statusEffectModel, senderModel, receiverModel, statusEffectDirection);
            HasFinished = false;
        }

        public override void Update()
        {
            if (!HasFinished)
            {
                if (CheckStopAffect(affectedModel))
                    Stop();
            }
        }

        public override void Stop()
        {
            FinishAffect(affectedModel);
            HasFinished = true;
            base.Stop();
        }

        protected virtual void FinishAffect(CharacterModel receiverModel) { }

        protected virtual bool CheckStopAffect(CharacterModel receiverModel)
            => false;

        #endregion Class methods
    }
}