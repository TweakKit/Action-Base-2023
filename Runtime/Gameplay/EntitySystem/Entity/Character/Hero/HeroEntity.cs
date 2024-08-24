using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public partial class HeroEntity : CharacterEntity<HeroModel>
    {
        #region Class Methods

        protected override void RunUpdate()
        {
            base.RunUpdate();
            switch (ownerModel.CharacterState)
            {
                case CharacterState.Idle:
                    PartialUpdateIdle();
                    break;

                case CharacterState.Move:
                    PartialUpdateMove();
                    break;

                case CharacterState.Attack:
                    PartialUpdateAttack();
                    break;

                case CharacterState.StandStill:
                    PartialUpdateStandStill();
                    break;

                case CharacterState.RefindTarget:
                    PartialUpdateRefindTarget();
                    break;

                case CharacterState.HardCC:
                    break;
            }
        }

        protected override void ExecuteValidate()
        {
            base.ExecuteValidate();
            PartialValidateStandStill();
            PartialValidateRefindTarget();
        }

        protected override void ExecuteInitialize()
        {
            base.ExecuteInitialize();
            PartialInitializeStandStill();
            PartialInitializeRefindTarget();
        }

        protected override void ExecuteDispose()
        {
            base.ExecuteDispose();
            PartialDisposeStandStill();
            PartialDisposeRefindTarget();
        }

        // -------------------------------- Validate Behaviors ------------------------------
        private partial void PartialValidateStandStill();
        private partial void PartialValidateRefindTarget();
        // ----------------------------------------------------------------------------------

        // -------------------------------- Initialize Behaviors ----------------------------
        private partial void PartialInitializeStandStill();
        private partial void PartialInitializeRefindTarget();
        // ----------------------------------------------------------------------------------

        // -------------------------------- Dispose Behaviors -------------------------------
        private partial void PartialDisposeStandStill();
        private partial void PartialDisposeRefindTarget();
        // ----------------------------------------------------------------------------------

        // -------------------------------- Update Behaviors -------------------------------
        private partial void PartialUpdateStandStill();
        private partial void PartialUpdateRefindTarget();
        // ----------------------------------------------------------------------------------

        #endregion Class Methods
    }
}