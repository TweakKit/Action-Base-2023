using System;
using System.Threading;
using UnityEngine;
using Runtime.Definition;
using Runtime.Extensions;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract partial class CharacterEntity<T> : Entity<T> where T : CharacterModel
    {
        #region Members

        protected const string GRAPHICS = "graphics";
        protected static readonly float ShowHitEffectColorDuration = 1/10f;
        protected GameObject graphics;
        protected bool isShowingGetHurt;
        protected ICharacterAnimationPlayer characterAnimationPlayer;
        protected CancellationTokenSource animateCancellationTokenSource;
        protected Color appearanceNormalSkinColor;
        protected Color appearanceGetHitSkinColor;

        #endregion Members

        #region Class Methods

        protected virtual partial void PartialValidateAnimate()
        {
            graphics = transform.FindChildGameObject(GRAPHICS);
            if (graphics == null)
            {
                Debug.LogError("Graphics name is not mapped!");
                return;
            }
        }

        protected virtual partial void PartialInitializeAnimate()
        {
            characterAnimationPlayer = transform.GetComponentInChildren<ICharacterAnimationPlayer>(true);
#if UNITY_EDITOR
            if (characterAnimationPlayer == null)
            {
                Debug.LogError($"Require a character animation player for this behavior!");
                return;
            }
#endif
            isShowingGetHurt = false;
            characterAnimationPlayer.Init();
            graphics = transform.FindChildGameObject(GRAPHICS);
            graphics.transform.localScale = new Vector2(1, 1);
            SetUpAppearanceColorSkin();
            OnDirectionChangedOnAnimate();
            SetAnimation(CharacterAnimationState.Idle);
            characterAnimationPlayer.TintColor(appearanceNormalSkinColor);
            ownerModel.MovementChangedEvent += OnMovementChangedOnAnimate;
            ownerModel.DirectionChangedEvent += OnDirectionChangedOnAnimate;
            ownerModel.ReactionChangedEvent += OnReactionChangedOnAnimate;
            ownerModel.HealthChangedEvent += OnHealthChangedOnAnimate;
            ownerModel.ShieldChangedEvent += OnShieldChangedOnAnimate;
            ownerModel.HardCCImpactedEvent += OnHardCCImpactedOnAnimate;
            animateCancellationTokenSource = new CancellationTokenSource();
        }

        protected virtual partial void PartialDisposeAnimate()
        {
            characterAnimationPlayer.Pause();
            if (animateCancellationTokenSource != null)
            {
                animateCancellationTokenSource.Cancel();
                animateCancellationTokenSource.Dispose();
                animateCancellationTokenSource = null;
            }
        }

        protected void OnMovementChangedOnAnimate()
        {
            if (ownerModel.HasMoveInput)
                SetAnimation(CharacterAnimationState.Move);
            else
                SetAnimation(CharacterAnimationState.Idle);
        }

        protected void OnDirectionChangedOnAnimate()
        {
            if (ownerModel.FaceRight)
                graphics.transform.localScale = new Vector2(-1, 1);
            else
                graphics.transform.localScale = new Vector2(1, 1);
        }

        protected void OnReactionChangedOnAnimate(CharacterReactionType characterReactionType)
        {
            switch (characterReactionType)
            {
                case CharacterReactionType.JustIdle:
                case CharacterReactionType.JustStandStill:
                case CharacterReactionType.JustRefindTarget:
                    SetAnimation(CharacterAnimationState.Idle);
                    break;

                case CharacterReactionType.JustMove:
                    if (ownerModel.HasMoveInput)
                        SetAnimation(CharacterAnimationState.Move);
                    else
                        SetAnimation(CharacterAnimationState.Idle);
                    break;

                case CharacterReactionType.JustDie:
                    SetAnimation(CharacterAnimationState.Die);
                    break;

                case CharacterReactionType.JustAnimateUnscaled:
                    characterAnimationPlayer.SetUnscaled(true);
                    break;

                case CharacterReactionType.JustResetAnimateUnscaled:
                    characterAnimationPlayer.SetUnscaled(false);
                    break;
            }
        }

        protected void OnShieldChangedOnAnimate(float deltaHp, DamageSource damageSource)
        {
            if (deltaHp < 0)
            {
                if (!isShowingGetHurt)
                {
                    if (animateCancellationTokenSource != null)
                        ShowHitEffectAsync(2, ShowHitEffectColorDuration, animateCancellationTokenSource.Token).Forget();
                }
            }
        }
        
        protected void OnHealthChangedOnAnimate(float deltaHp, DamageSource damageSource)
        {
            if (deltaHp < 0)
            {
                if (!isShowingGetHurt)
                {
                    if (animateCancellationTokenSource != null)
                        ShowHitEffectAsync(2, ShowHitEffectColorDuration, animateCancellationTokenSource.Token).Forget();
                }
            }
        }

        protected void OnHardCCImpactedOnAnimate(StatusEffectType statusEffectType)
            => SetAnimation(CharacterAnimationState.Idle);

        protected virtual void SetUpAppearanceColorSkin()
        {
            appearanceNormalSkinColor = Constant.APPEARANCE_ENTITY_NORMAL_SKIN_COLOR;
            appearanceGetHitSkinColor = Constant.APPEARANCE_ENTITY_GET_HIT_SKIN_COLOR;
        }

        protected void SetAnimation(CharacterAnimationState state)
            => characterAnimationPlayer.Play(state);

        protected virtual async UniTask ShowHitEffectAsync(int showHitEffectColorTimes, float showHitEffectColorDuration, CancellationToken cancellationToken)
        {
            isShowingGetHurt = true;
            int currentShowHitEffectColorTimes = 0;
            while (currentShowHitEffectColorTimes < showHitEffectColorTimes)
            {
                currentShowHitEffectColorTimes++;
                characterAnimationPlayer.TintColor(appearanceGetHitSkinColor);
                await UniTask.Delay(TimeSpan.FromSeconds(showHitEffectColorDuration), cancellationToken: cancellationToken);
                characterAnimationPlayer.TintColor(appearanceNormalSkinColor);
                await UniTask.Delay(TimeSpan.FromSeconds(showHitEffectColorDuration), cancellationToken: cancellationToken);
            }
            characterAnimationPlayer.TintColor(appearanceNormalSkinColor);
            isShowingGetHurt = false;
        }

        #endregion Class Methods
    }
}