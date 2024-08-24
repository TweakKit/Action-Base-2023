using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class AreaAffectModel
    {
        #region Members

        #endregion  Members

        #region Properties

        public UnityEngine.Vector2 Position { get; private set; }
        public float Duration { get; private set; }
        public EntityType TeammateTargetType { get; private set; }
        public EntityType RivalsTargetType { get; private set; }

        #endregion Properties

        #region Class Methods

        public AreaAffectModel(UnityEngine.Vector2 postion, float duration, EntityType teammateTargetType, EntityType rivalsTargetType)
        {
            Position = postion;
            Duration = duration;
            TeammateTargetType = teammateTargetType;
            RivalsTargetType = rivalsTargetType;
        }

        #endregion Class Methods
    }
}