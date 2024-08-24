using UnityEngine;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IStatusEffect
    {
        #region Properties

        StatusEffectType StatusEffectType { get; }
        StatusEffectModel StatusEffectModel { get; }
        bool HasFinished { get; }

        #endregion Properties

        #region Interface Methods

        void Init(StatusEffectModel modifierModel, EntityModel senderModel, CharacterModel receiverModel, Vector2 statusEffectDirection);
        public abstract void Update();
        public abstract void Stop();

        #endregion Interface Methods
    }

    public class AffectedStatusEffectInfo
    {
        #region Members

        public StatusEffectModel[] affectedStatusEffectModels;
        public StatusEffectType[] cutOffStatusEffectTypes;
        public EntityModel creatorModel;

        #endregion Members

        #region Class Methods

        public AffectedStatusEffectInfo(StatusEffectModel[] affectedStatusEffectModels, StatusEffectType[] cutOffStatusEffectTypes, EntityModel creatorModel)
        {
            this.affectedStatusEffectModels = affectedStatusEffectModels;
            this.cutOffStatusEffectTypes = cutOffStatusEffectTypes;
            this.creatorModel = creatorModel;
        }

        #endregion Class Methods
    }
}