using System;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class ObjectModel : EntityModel
    {
        #region Properties

        public Action<float> HealthChangedEvent { get; set; }
        public Action DestroyEvent { get; set; }

        #endregion Properties

        #region Class Methods

        protected override void InitEvents()
        {
            base.InitEvents();
            HealthChangedEvent = _ => { };
            DestroyEvent = () => { };
        }

        #endregion Class Methods
    }
}