using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    /// <summary>
    /// One shot status effect affects the target right off the bat.
    /// </summary>
    public abstract class OneShotStatusEffect<T> : StatusEffect<T>, IStatusEffect where T : StatusEffectModel
    {
        #region Members

        protected CharacterModel affectedModel;

        #endregion Members

        #region Class Methods

        public override void Init(StatusEffectModel statusEffectModel, EntityModel senderModel, CharacterModel receiverModel, Vector2 statusEffectDirection)
        {
            base.Init(statusEffectModel, senderModel, receiverModel, statusEffectDirection);
            affectedModel = receiverModel;
            HasFinished = true;
            StartAffect(affectedModel);
        }

        public override void Update() { }
        protected virtual void StartAffect(CharacterModel receiverModel) { }

        #endregion Class methods
    }
}