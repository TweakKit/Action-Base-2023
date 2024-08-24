using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public static class AttackStrategyFactory
    {
        #region Class Methods

        public static IAttackStrategy GetAttackStrategy(bool isHero, AttackType attackType)
        {
            switch (attackType)
            {
                case AttackType.MeleeAttack:
                    if (isHero)
                        return new MeleeInteractAttackStrategy();
                    else
                        return new MeleeAttackStrategy();

                case AttackType.RangedAttack:
                    if (isHero)
                        return new RangedInteractAttackStrategy();
                    else
                        return new RangedAttackStrategy();

                case AttackType.MeleeDamageBehindAttack:
                    if (isHero)
                        return new MeleeDamageBehindInteractAttackStrategy();
                    else
                        return new MeleeDamageBehindAttackStrategy();

                case AttackType.RangedEmptyProjectileAttack:
                    if (isHero)
                        return new RangedEmptyProjectileInteractAttackStrategy();
                    else
                        return new RangedEmptyProjectileAttackStrategy();
                
                case AttackType.RangedLazerProjectileAttack:
                    if(isHero)
                        return new RangedLazerProjectileInteractAttackStrategy();
                    else
                        return new RangedLazerProjectileAttackStrategy();

                case AttackType.MeleeAutoDestroyAttack:
                    return new MeleeAutoDestroyAttackStrategy();

                case AttackType.MeleeSpawnFlailAttack:
                    return new MeleeSpawnFlailAttackStrategy();

                case AttackType.MeleeAOEAttack:
                    return new MeleeAOEAttackStrategy();

                case AttackType.GoldenMimicAttack:
                    return new GoldenMimicAttackStrategy();

                case AttackType.RangedSpawnImpactAttack:
                    return new RangedSpawnImpactAttackStrategy();

                case AttackType.MeleeSpawnDarkFlailAttack:
                    return new MeleeSpawnDarkFlailAttackStrategy();

                case AttackType.RangedBurnEffectAttack:
                    return new RangedBurnEffectAttackStrategy();

                case AttackType.RangedPenetrateProjectileAttack:
                    if (isHero)
                    {
                        return new RangedPenetrateProjectileInteractAttackStrategy();
                    }

                    return new RangedPenetrateProjectileAttackStrategy();

                default:
                    return null;
            }
        }

        #endregion Class Methods
    }
}