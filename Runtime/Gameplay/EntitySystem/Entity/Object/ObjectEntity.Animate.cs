using System;
using System.Threading;
using UnityEngine;
using Runtime.Extensions;
using Runtime.Definition;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class ObjectEntity : Entity<ObjectModel>
    {
        #region Members

        protected bool isShowingGetDamaged;
        protected const string GRAPHICS = "graphics";
        protected static readonly float ShowHitEffectColorDuration = 1/10f;
        protected SpriteRenderer graphicsRenderer;
        protected CancellationTokenSource effectCancellationTokenSource;

        #endregion Members

        #region Class Methods

        private partial void PartialValidateAnimate()
        {
            var graphics = transform.FindChildGameObject(GRAPHICS);
            if (graphics == null)
            {
                Debug.LogError("Graphics name is not mapped!");
                return;
            }
            else graphicsRenderer = graphics.GetComponentInChildren<SpriteRenderer>(true);
        }

        private partial void PartialInitializeAnimate()
        {
            isShowingGetDamaged = false;
            var graphics = transform.FindChildGameObject(GRAPHICS);
            graphicsRenderer = graphics.GetComponentInChildren<SpriteRenderer>(true);
            graphicsRenderer.material.SetColor(Constant.HIT_MATERIAL_COLOR_PROPERTY, Constant.APPEARANCE_ENTITY_NORMAL_SKIN_COLOR);
            ownerModel.HealthChangedEvent += OnHealthChanged;
            effectCancellationTokenSource = new CancellationTokenSource();
        }

        private partial void PartialDisposeAnimate()
        {
            if (effectCancellationTokenSource != null)
            {
                effectCancellationTokenSource.Cancel();
                effectCancellationTokenSource.Dispose();
                effectCancellationTokenSource = null;
            }
        }

        private void OnHealthChanged(float deltaHp)
        {
            if (deltaHp < 0)
            {
                if (!isShowingGetDamaged)
                {
                    if (effectCancellationTokenSource != null)
                        ShowHitEffectAsync(1, ShowHitEffectColorDuration, effectCancellationTokenSource.Token).Forget();
                }
            }
        }

        private async UniTask ShowHitEffectAsync(int showHitEffectColorTimes, float showHitEffectColorDuration, CancellationToken cancellationToken)
        {
            isShowingGetDamaged = true;
            int currentShowHitEffectColorTimes = 0;
            while (currentShowHitEffectColorTimes < showHitEffectColorTimes)
            {
                currentShowHitEffectColorTimes++;
                graphicsRenderer.material.SetColor(Constant.HIT_MATERIAL_COLOR_PROPERTY, Constant.APPEARANCE_ENTITY_GET_HIT_SKIN_COLOR);
                await UniTask.Delay(TimeSpan.FromSeconds(showHitEffectColorDuration), ignoreTimeScale: true, cancellationToken: cancellationToken);
                graphicsRenderer.material.SetColor(Constant.HIT_MATERIAL_COLOR_PROPERTY, Constant.APPEARANCE_ENTITY_NORMAL_SKIN_COLOR);
                await UniTask.Delay(TimeSpan.FromSeconds(showHitEffectColorDuration), ignoreTimeScale: true, cancellationToken: cancellationToken);
            }
            graphicsRenderer.material.SetColor(Constant.HIT_MATERIAL_COLOR_PROPERTY, Constant.APPEARANCE_ENTITY_NORMAL_SKIN_COLOR);
            isShowingGetDamaged = false;
        }

        #endregion Class Methods
    }
}