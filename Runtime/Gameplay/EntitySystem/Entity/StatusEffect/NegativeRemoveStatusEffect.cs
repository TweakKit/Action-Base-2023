using System;
using UnityEngine;
using Runtime.Gameplay.Manager;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public class NegativeRemoveStatusEffect : OneShotStatusEffect<NegativeRemoveStatusEffectModel>
    {
        #region Class Methods

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
        }

        #endregion Class Methods
    }
}