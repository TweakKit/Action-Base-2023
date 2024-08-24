using UnityEngine;
using UnityEngine.AddressableAssets;
using Runtime.Definition;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class PVPHeroEntity : HeroEntity
    {
        #region Class Methods

        protected override void PartialInitializeAnimate()
        {
            base.PartialInitializeAnimate();
            SetUpSkinSpriteOutlineMaterial();
        }

        protected override void SetUpAppearanceColorSkin()
        {
            if (ownerModel.IsHeroBoss)
            {
                appearanceNormalSkinColor = Constant.APPEARANCE_HERO_BOSS_NORMAL_SKIN_COLOR;
                appearanceGetHitSkinColor = Constant.APPEARANCE_HERO_BOSS_GET_HIT_SKIN_COLOR;
            }
            else
            {
                appearanceNormalSkinColor = Constant.APPEARANCE_ENTITY_NORMAL_SKIN_COLOR;
                appearanceGetHitSkinColor = Constant.APPEARANCE_ENTITY_GET_HIT_SKIN_COLOR;
            }
        }

        private void SetUpSkinSpriteOutlineMaterial()
            => LoadSkinSpriteMaterialAsync().Forget();

        private async UniTask LoadSkinSpriteMaterialAsync()
        {
            var spriteMaterialName = ownerModel.IsHeroBoss
                                   ? Constant.SPRITE_OUTLINE_MATERIAL_NAME
                                   : Constant.SPRITE_COLOR_MATERIAL_NAME;
            var spriteMaterial = await Addressables.LoadAssetAsync<Material>(spriteMaterialName)
                                                   .WithCancellation(animateCancellationTokenSource.Token);
            var skinSpriteRender = graphics.GetComponentInChildren<SpriteRenderer>();
            skinSpriteRender.material = spriteMaterial;
            characterAnimationPlayer.TintColor(appearanceNormalSkinColor);
        }

        #endregion Class Methods
    }
}