using UnityEngine;
using Runtime.Extensions;
using Runtime.Gameplay.Visual;
using Runtime.Definition;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract partial class CharacterEntity<T> : Entity<T> where T : CharacterModel
    {
        #region Members

        private const string HUD_NAME = "character_hud";
        private CharacterHUD _characterHUD;

        #endregion Members

        #region Class Methods

        protected virtual partial void PartialDisposeHUD() { }

        protected virtual partial void PartialValidateHUD()
        {
            var characterHUDGameObject = transform.FindChildTransform(HUD_NAME);
            if (characterHUDGameObject == null)
            {
                Debug.LogError("Character HUD name is not mapped!");
                return;
            }

            _characterHUD = characterHUDGameObject.GetComponent<CharacterHUD>();
            if (_characterHUD == null)
            {
                Debug.LogError("Require a Character HUD component!");
                return;
            }
        }

        protected virtual partial void PartialInitializeHUD()
        {
            _characterHUD = transform.FindChildGameObject(HUD_NAME).GetComponent<CharacterHUD>();
            ownerModel.HealthChangedEvent += OnHealthChangedOnHUD;
            ownerModel.ShieldChangedEvent += OnShieldChangedOnHUD;
            ownerModel.StatChangedEvent += OnStatChanged;
            ownerModel.ReactionChangedEvent += OnReactionChangedOnHUD;
            ownerModel.DeathEvent += OnDeathOnHUD;
            InitHUD();
        }

        protected virtual void InitHUD()
            => _characterHUD.Init(ownerModel.EntityType, ownerModel.IsHeroBoss);
        
        private void OnStatChanged(StatType statType, float value)
        {
            if(statType == StatType.HealthPoint)
                _characterHUD.UpdateHealthBar(ownerModel.CurrentHp, ownerModel.MaxHp);
            else if (statType == StatType.ShieldPoint)
                _characterHUD.UpdateShieldBar(ownerModel.CurrentDefense, ownerModel.MaxDefense);
        }

        protected virtual void OnHealthChangedOnHUD(float deltaHp, DamageSource damageSource)
        {
#if UNITY_EDITOR
            Debug.Log($"{ownerModel.EntityId}-{ownerModel.EntityUId}: {deltaHp} HP by {damageSource}");
#endif
            ShowDamageVisualAsync(deltaHp, damageSource == DamageSource.FromCritAttack || damageSource == DamageSource.FromCritSkill);
            _characterHUD.UpdateHealthBar(ownerModel.CurrentHp, ownerModel.MaxHp);
        }
        
        protected virtual void OnShieldChangedOnHUD(float deltaShield, DamageSource damageSource)
        {
#if UNITY_EDITOR
            Debug.Log($"{ownerModel.EntityId}-{ownerModel.EntityUId}: {deltaShield} Shield by {damageSource}");
#endif
            ShowDamageVisualAsync(deltaShield, damageSource == DamageSource.FromCritAttack || damageSource == DamageSource.FromCritSkill);
            _characterHUD.UpdateShieldBar(ownerModel.CurrentDefense, ownerModel.MaxDefense);
        }

        protected virtual void OnReactionChangedOnHUD(CharacterReactionType characterReactionType)
        {
            switch (characterReactionType)
            {
                case CharacterReactionType.JustMissDamage:
                    var damageVisualKey = ownerModel.EntityType.IsHero() ? VFXKey.HERO_DAMAGE_VISUAL: VFXKey.ENEMY_DAMAGE_VISUAL;
                    GameplayVisualController.Instance.DislayDamageVisualMissed(damageVisualKey,
                                                                               ownerModel.TopPosition,
                                                                               this.GetCancellationTokenOnDestroy()).Forget();
                    break;
            }
        }

        protected virtual void ShowDamageVisualAsync(float deltaHp, bool isCrit)
        {
            int damageValue = Mathf.RoundToInt(deltaHp);
            var damageVisualKey = ownerModel.EntityType.IsHero() ? VFXKey.HERO_DAMAGE_VISUAL: VFXKey.ENEMY_DAMAGE_VISUAL;
            GameplayVisualController.Instance.DislayDamageVisual(damageVisualKey,
                                                                 damageValue,
                                                                 isCrit,
                                                                 ownerModel.TopPosition,
                                                                 this.GetCancellationTokenOnDestroy()).Forget();
        }

        protected virtual void OnDeathOnHUD(DamageSource damageSource)
            => _characterHUD.SetVisibility(false);

        #endregion Class Methdos
    }
}