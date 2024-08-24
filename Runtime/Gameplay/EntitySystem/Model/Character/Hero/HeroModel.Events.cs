using System;
using System.Collections.Generic;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class HeroModel : CharacterModel
    {
        #region Properties

        public Action<MovementStrategyType> MovementStrategyChangedEvent { get; set; }
        public Action<List<SkillModel>> SkillStrategyUpdatedEvent { get; set; }

        #endregion Properties

        #region Class Methods

        protected override void InitEvents()
        {
            base.InitEvents();
            MovementStrategyChangedEvent = _ => { };
            SkillStrategyUpdatedEvent = _ => { };
        }

        #endregion Class Methods
    }
}