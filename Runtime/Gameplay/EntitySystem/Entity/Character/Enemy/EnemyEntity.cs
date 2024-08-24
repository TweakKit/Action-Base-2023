using UnityEngine;
using Runtime.Definition;
using Runtime.Gameplay.Manager;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public partial class EnemyEntity : CharacterEntity<EnemyModel>
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

                case CharacterState.HardCC:
                    break;
            }
        }

        protected override void ExecuteValidate()
        {
            base.ExecuteValidate();
            PartialValidateDetectTarget();
        }

        protected override void ExecuteInitialize()
        {
            base.ExecuteInitialize();
            PartialInitializeDetectTarget();
        }

        protected override void ExecuteDispose()
        {
            base.ExecuteDispose();
            PartialDisposeDetectTarget();
        }

        // -------------------------------- Validate Behaviors ------------------------------
        private partial void PartialValidateDetectTarget();
        // ----------------------------------------------------------------------------------

        // -------------------------------- Initialize Behaviors ----------------------------
        private partial void PartialInitializeDetectTarget();
        // ----------------------------------------------------------------------------------

        // -------------------------------- Dispose Behaviors -------------------------------
        private partial void PartialDisposeDetectTarget();
        // ----------------------------------------------------------------------------------

        public override void Build(EntityModel model, Vector3 position)
        {
            base.Build(model, position);
            this.ownerModel.SetCurrentBattleIndex(EntitiesManager.CurrentBattleIndex);
        }

        public override void SetActive(bool isActive)
        {
            base.SetActive(isActive);
            if (isActive)
            {
                if (this.ownerModel.CurrentBattleIndex < EntitiesManager.CurrentBattleIndex)
                {
                    this.ownerModel.SetCurrentBattleIndex(EntitiesManager.CurrentBattleIndex);
                    ownerModel.RestoreHp();
                }
            }
        }

        public override void SetAnimationUnscaled(bool isUnscaled)
        {
            base.SetAnimationUnscaled(isUnscaled);
            var reactionType = isUnscaled
                             ? CharacterReactionType.JustAnimateUnscaled
                             : CharacterReactionType.JustResetAnimateUnscaled;
            ownerModel.ReactionChangedEvent.Invoke(reactionType);
        }

        protected override void SetUpScale()
        {
            transform.localScale = ownerModel.IsElite
                                 ? Constant.ELITE_SCALE
                                 : Vector3.one;
        }

        #endregion Class Methods
    }
}