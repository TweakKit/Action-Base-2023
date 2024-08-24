using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public partial class ObjectEntity : Entity<ObjectModel>
    {
        #region Class Methods

        protected override void ExecuteValidate()
        {
            base.ExecuteValidate();
            PartialValidateInteractable();
            PartialValidateAnimate();
            PartialValidateDestroy();
            PartialValidateHUD();
        }

        protected override void ExecuteInitialize()
        {
            base.ExecuteInitialize();
            PartialInitializeInteractable();
            PartialInitializeAnimate();
            PartialInitializeDestroy();
            PartialInitializeHUD();
        }

        protected override void ExecuteDispose()
        {
            base.ExecuteDispose();
            PartialDisposeInteractable();
            PartialDisposeAnimate();
            PartialDisposeDestroy();
            PartialDisposeHUD();
        }

        // -------------------------------- Validate Behaviors ------------------------------
        private partial void PartialValidateInteractable();
        private partial void PartialValidateAnimate();
        private partial void PartialValidateDestroy();
        private partial void PartialValidateHUD();
        // ----------------------------------------------------------------------------------

        // -------------------------------- Initialize Behaviors ----------------------------
        private partial void PartialInitializeInteractable();
        private partial void PartialInitializeAnimate();
        private partial void PartialInitializeDestroy();
        private partial void PartialInitializeHUD();
        // ----------------------------------------------------------------------------------

        // -------------------------------- Dispose Behaviors -------------------------------
        private partial void PartialDisposeInteractable();
        private partial void PartialDisposeAnimate();
        private partial void PartialDisposeDestroy();
        private partial void PartialDisposeHUD();
        // ----------------------------------------------------------------------------------

        #endregion Class Methods
    }
}