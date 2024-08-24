using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class AxeStrikeSkillStrategy : SkillStrategy<AxeStrikeSkillModel>
    {
        #region Class Methods

        protected override void InitActions(AxeStrikeSkillModel skillModel)
        {
            var axeStrikeSkillAction = new AxeStrikeSkillAction();
            skillActions.Add(axeStrikeSkillAction);
            axeStrikeSkillAction.Init(creatorModel,
                                      creatorTransform,
                                      skillModel.SkillType,
                                      skillModel.TargetType,
                                      SkillActionPhase.Cast,
                                      skillModel.CastRange,
                                      skillModel.DamageFactorValue);
        }

        #endregion Class Methods
    }
}