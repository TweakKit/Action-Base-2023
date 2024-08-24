using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public partial class PVPHeroEntity : HeroEntity
    {
        #region Class Methods

        protected override void ExecuteValidate()
        {
            base.ExecuteValidate();
            PartialValidateDetectTarget();
            PartialValidateStatistics();
        }

        protected override void ExecuteInitialize()
        {
            base.ExecuteInitialize();
            PartialInitializeDetectTarget();
            PartialInitializeStatistics();
        }

        protected override void ExecuteDispose()
        {
            base.ExecuteDispose();
            PartialDisposeDetectTarget();
            PartialDisposeStatistics();
        }

        // -------------------------------- Validate Behaviors ------------------------------
        private partial void PartialValidateDetectTarget();
        private partial void PartialValidateStatistics();
        // ----------------------------------------------------------------------------------

        // -------------------------------- Initialize Behaviors ----------------------------
        private partial void PartialInitializeDetectTarget();
        private partial void PartialInitializeStatistics();
        // ----------------------------------------------------------------------------------

        // -------------------------------- Dispose Behaviors -------------------------------
        private partial void PartialDisposeDetectTarget();
        private partial void PartialDisposeStatistics();
        // ----------------------------------------------------------------------------------

        #endregion Class Methods
    }
}