using System;
using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public abstract class StatusEffect<T> : IStatusEffect where T : StatusEffectModel
    {
        #region Members

        protected T ownerModel;

        #endregion Members

        #region Properties

        public StatusEffectModel StatusEffectModel => ownerModel;
        public bool HasFinished { get; protected set; }
        public StatusEffectType StatusEffectType => ownerModel.StatusEffectType;

        #endregion Properties

        #region Class methods

        public virtual void Init(StatusEffectModel statusEffectModel, EntityModel senderModel, CharacterModel receiverModel, Vector2 statusEffectDirection)
        {
            ownerModel = statusEffectModel as T;
            InitData(senderModel);
        }

        public abstract void Update();
        public virtual void Stop() { }
        protected virtual void InitData(EntityModel senderModel) { }

        #endregion Class methods
    }
}