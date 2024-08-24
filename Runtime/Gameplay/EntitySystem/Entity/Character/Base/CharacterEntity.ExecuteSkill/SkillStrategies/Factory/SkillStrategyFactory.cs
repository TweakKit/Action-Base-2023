using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public static class SkillStrategyFactory
    {
        #region Class Methods

        public static ISkillStrategy GetSkillStrategy(SkillType skillType)
        {
            switch (skillType)
            {
                case SkillType.AxeStrike:
                    return new AxeStrikeSkillStrategy();
            }

            return null;
        }

        #endregion Class Methods
    }
}