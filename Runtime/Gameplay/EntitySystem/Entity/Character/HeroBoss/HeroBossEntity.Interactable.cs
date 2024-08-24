using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class HeroBossEntity : BossEntity
    {
        #region Class Methods

        public override void GetAffected(EntityModel senderModel, StatusEffectModel[] statusEffectModels, Vector2 affectDirection)
        {
            foreach (var statusEffectModel in statusEffectModels)
                CauseAffect(statusEffectModel, senderModel, affectDirection);
        }

        #endregion Class Methods
    }
}