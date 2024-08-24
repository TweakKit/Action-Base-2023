using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    #region Class Methods

    public static class StatusEffectFactory
    {
        public static IStatusEffect GetStatusEffect(StatusEffectType statusEffectType)
        {
            switch (statusEffectType)
            {
                case StatusEffectType.Regen:
                    return new RegenStatusEffect();

                case StatusEffectType.Haste:
                    return new HasteStatusEffect();

                case StatusEffectType.Quick:
                    return new QuickStatusEffect();

                case StatusEffectType.KnockUp:
                    return new KnockUpStatusEffect();

                case StatusEffectType.Heal:
                    return new HealStatusEffect();

                case StatusEffectType.HealAttack:
                    return new HealAttackStatusEffect();

                case StatusEffectType.BleedAttack:
                    return new BleedAttackStatusEffect();

                case StatusEffectType.PoisonAttack:
                    return new PoisonAttackStatusEffect();

                case StatusEffectType.CritChanceBuff:
                    return new CritChanceBuffStatusEffect();

                case StatusEffectType.Stun:
                    return new StunStatusEffect();

                case StatusEffectType.EvasionBuff:
                    return new EvasionBuffStatusEffect();

                case StatusEffectType.AttackBuff:
                    return new AttackBuffStatusEffect();

                case StatusEffectType.BurnAttack:
                    return new BurnAttackStatusEffect();

                case StatusEffectType.Chill:
                    return new ChillStatusEffect();

                case StatusEffectType.Freeze:
                    return new FreezeStatusEffect();

                case StatusEffectType.DamageReductionBuff:
                    return new DamageReductionStatusEffect();

                case StatusEffectType.FixedDamageReductionBuff:
                    return new FixedDamageReductionStatusEffect();

                case StatusEffectType.NegativeStatusEffectRemove:
                    return new NegativeRemoveStatusEffect();

                case StatusEffectType.Slow:
                    return new SlowStatusEffect();

                case StatusEffectType.HealingAttackDuration:
                    return new HealingAttackDurationStatusEffect();

                case StatusEffectType.DamageReductionNonDuration:
                    return new DamageReductionNonDurationStatusEffect();

                case StatusEffectType.Taunt:
                    return new TauntStatusEffect();

                case StatusEffectType.FixedDamageReductionNonDuration:
                    return new FixedDamageReductionNonDurationStatusEffect();

                case StatusEffectType.Berserker:
                    return new BerserkerStatusEffect();

                case StatusEffectType.Invincibility:
                    return new InvincibilityStatusEffect();

                case StatusEffectType.DecreaseTakeHp:
                    return new DecreaseTakeHpStatusEffect();

                case StatusEffectType.Gargoyle:
                    return new GargoyleStatusEffect();

                case StatusEffectType.GetMoreDamage:
                    return new GetMoreDamageStatusEffect();

                case StatusEffectType.Goku:
                    return new GokuStatusEffect();

                case StatusEffectType.PullToCenter:
                    return new PullToCenterEffect();
                
                case StatusEffectType.Shield:
                    return new ShieldStatusEffect();

                default:
                    return null;
            }
        }

        #endregion Class Methods
    }
}