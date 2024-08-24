using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public abstract partial class CharacterEntity<T> : Entity<T> where T : CharacterModel
    {
        #region API Methods

        private void Update()
            => RunUpdate();

        #endregion API Methods

        #region Class Methods

        protected virtual void RunUpdate()
            => PartialUpdateInteractable();

        protected override void ExecuteValidate()
        {
            base.ExecuteValidate();
            PartialValidateIdle();
            PartialValidateMove();
            PartialValidateDie();
            PartialValidateAnimate();
            PartialValidateAttack();
            PartialValidateHUD();
            PartialValidateInteractable();
        }

        protected override void ExecuteInitialize()
        {
            base.ExecuteInitialize();
            PartialInitializeIdle();
            PartialInitializeMove();
            PartialInitializeDie();
            PartialInitializeAnimate();
            PartialInitializeAttack();
            PartialInitializeHUD();
            PartialInitializeInteractable();
        }

        protected override void ExecuteDispose()
        {
            base.ExecuteDispose();
            PartialDisposeIdle();
            PartialDisposeMove();
            PartialDisposeDie();
            PartialDisposeAnimate();
            PartialDisposeAttack();
            PartialDisposeHUD();
            PartialDisposeInteractable();
        }

        // -------------------------------- Validate Behaviors ------------------------------
        protected virtual partial void PartialValidateIdle();
        protected virtual partial void PartialValidateMove();
        protected virtual partial void PartialValidateDie();
        protected virtual partial void PartialValidateAnimate();
        protected virtual partial void PartialValidateAttack();
        protected virtual partial void PartialValidateHUD();
        protected virtual partial void PartialValidateInteractable();
        // ----------------------------------------------------------------------------------

        // -------------------------------- Initialize Behaviors ----------------------------
        protected virtual partial void PartialInitializeIdle();
        protected virtual partial void PartialInitializeMove();
        protected virtual partial void PartialInitializeDie();
        protected virtual partial void PartialInitializeAnimate();
        protected virtual partial void PartialInitializeAttack();
        protected virtual partial void PartialInitializeHUD();
        protected virtual partial void PartialInitializeInteractable();
        // ----------------------------------------------------------------------------------

        // -------------------------------- Dispose Behaviors -------------------------------
        protected virtual partial void PartialDisposeIdle();
        protected virtual partial void PartialDisposeMove();
        protected virtual partial void PartialDisposeDie();
        protected virtual partial void PartialDisposeAnimate();
        protected virtual partial void PartialDisposeAttack();
        protected virtual partial void PartialDisposeHUD();
        protected virtual partial void PartialDisposeInteractable();
        // ----------------------------------------------------------------------------------

        // -------------------------------- Update Behaviors --------------------------------
        protected virtual partial void PartialUpdateIdle();
        protected virtual partial void PartialUpdateMove();
        protected virtual partial void PartialUpdateAttack();
        protected virtual partial void PartialUpdateInteractable();
        // ----------------------------------------------------------------------------------

        #endregion Class Methods
    }
}