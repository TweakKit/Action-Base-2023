using UnityEngine;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class BossEntity : EnemyEntity
    {
        #region Class Methods

        public override void GetAffected(EntityModel senderModel, StatusEffectModel[] statusEffectModels, Vector2 affectDirection)
        {
            foreach (var statusEffectModel in statusEffectModels)
            {
                if (Constant.CanBossReceiveStatusEffect(statusEffectModel.StatusEffectType))
                    CauseAffect(statusEffectModel, senderModel, affectDirection);
            }
        }

        #endregion Class Methods
    }
}