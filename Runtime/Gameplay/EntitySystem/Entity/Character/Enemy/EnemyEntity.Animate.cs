using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EnemyEntity : CharacterEntity<EnemyModel>
    {
        #region Class Methods

        protected override void SetUpAppearanceColorSkin()
        {
            appearanceNormalSkinColor = Constant.APPEARANCE_ENTITY_NORMAL_SKIN_COLOR;
            appearanceGetHitSkinColor = Constant.APPEARANCE_ENEMY_GET_HIT_SKIN_COLOR;
        }

        #endregion Class Methods
    }
}