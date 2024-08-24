using UnityEngine;
using UnityEngine.AddressableAssets;
using Runtime.Definition;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class HeroBossEntity : BossEntity
    {
        #region Class Methods

        protected override void PartialInitializeAnimate()
        {
            base.PartialInitializeAnimate();
            SetUpSkinSpriteMaterial();
        }

        protected override void SetUpAppearanceColorSkin()
        {
            appearanceNormalSkinColor = Constant.APPEARANCE_HERO_BOSS_NORMAL_SKIN_COLOR;
            appearanceGetHitSkinColor = Constant.APPEARANCE_HERO_BOSS_GET_HIT_SKIN_COLOR;
        }

        private void SetUpSkinSpriteMaterial()
            => LoadSkinSpriteMaterialAsync().Forget();

        private async UniTask LoadSkinSpriteMaterialAsync()
        {
            var spriteMaterialName = Constant.SPRITE_OUTLINE_MATERIAL_NAME;
            var spriteMaterial = await Addressables.LoadAssetAsync<Material>(spriteMaterialName)
                                                   .WithCancellation(animateCancellationTokenSource.Token);
            var skinSpriteRender = graphics.GetComponentInChildren<SpriteRenderer>();
            skinSpriteRender.material = spriteMaterial;
            characterAnimationPlayer.TintColor(appearanceNormalSkinColor);
        }

        #endregion Class Methods
    }
}