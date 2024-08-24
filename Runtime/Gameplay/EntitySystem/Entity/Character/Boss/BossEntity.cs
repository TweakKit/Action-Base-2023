using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class BossEntity : EnemyEntity
    {
        #region Class Methods

        protected override void SetUpScale()
        {
            transform.localScale = ownerModel.IsHeroBoss
                                 ? Constant.HERO_BOSS_SCALE
                                 : Vector3.one;
        }

        #endregion Class Methods
    }
}