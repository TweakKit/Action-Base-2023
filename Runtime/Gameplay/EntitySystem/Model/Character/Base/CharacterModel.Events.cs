using System;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public enum CharacterReactionType
    {
        JustDie,
        JustMove,
        justMoveInGroup,
        JustIdle,
        JustFinishAttack,
        JustStandStill,
        JustRefindTarget,
        JustMissDamage,
        JustAnimateUnscaled,
        JustResetAnimateUnscaled,
        JustSawHeroTeleported,
    }

    public abstract partial class CharacterModel : EntityModel
    {
        #region Properties

        public Action MovementChangedEvent { get; set; }
        public Action DirectionChangedEvent { get; set; }
        public Action MovePositionUpdatedEvent { get; set; }
        public Action<DamageSource> DeathEvent { get; set; }
        public Action<float, DamageSource> HealthChangedEvent { get; set; }
        public Action<float, DamageSource> ShieldChangedEvent { get; set; }
        public Action<CharacterReactionType> ReactionChangedEvent { get; set; }
        public Action<StatType, float> StatChangedEvent { get; set; }
        public Action<StatusEffectType> HardCCImpactedEvent { get; set; }

        #endregion Properties

        #region Class Methods

        protected override void InitEvents()
        {
            base.InitEvents();
            MovementChangedEvent = () => { };
            MovePositionUpdatedEvent = () => { };
            DirectionChangedEvent = () => { };
            DeathEvent = _ => { };
            HealthChangedEvent = (_, _) => { };
            ShieldChangedEvent = (_, _) => { };
            ReactionChangedEvent = _ => { };
            StatChangedEvent = (_, _) => { };
            HardCCImpactedEvent = _ => { };
        }

        #endregion Class Methods
    }
}