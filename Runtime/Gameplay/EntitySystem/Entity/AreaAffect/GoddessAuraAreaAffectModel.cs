using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class GoddessAuraAreaAffectModel : AreaAffectModel
    {
        #region Members

        #endregion  Members

        #region Properties

        public float TimeStepOneWave { get; private set; }

        #endregion Properties

        #region Class Methods

        public GoddessAuraAreaAffectModel(UnityEngine.Vector2 postion,
                                            float duration,
                                            EntityType teammateTargetType,
                                            EntityType rivalsTargetType,
                                            float timeStepOneWave = 1.0f) :
                                            base(postion, duration, teammateTargetType, rivalsTargetType)
         => TimeStepOneWave = timeStepOneWave;

        #endregion Class Methods
    }
}