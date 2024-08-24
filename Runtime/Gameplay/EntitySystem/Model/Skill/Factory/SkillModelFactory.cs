using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public static class SkillModelFactory
    {
        #region Class Methods

        public static SkillModel GetSkillModel(SkillType skillType, SkillData skillData)
        {
            switch (skillType)
            {
                case SkillType.HeavyStrike:
                    return new HeavyStrikeSkillModel(skillData);

                case SkillType.Encourage:
                    return new EncourageSkillModel(skillData);

                case SkillType.AxeStrike:
                    return new AxeStrikeSkillModel(skillData);

                case SkillType.RushAttack:
                    return new RushAttackSkillModel(skillData);

                case SkillType.HealingHand:
                    return new HealingHandSkillModel(skillData);

                case SkillType.WindStep:
                    return new WindStepSkillModel(skillData);

                case SkillType.WhirlStrike:
                    return new WhirlStrikeSkillModel(skillData);

                case SkillType.SpawnFlail:
                    return new SpawnFlailSkillModel(skillData);

                case SkillType.PikeThrust:
                    return new PikeThrustSkillModel(skillData);

                case SkillType.HealingOrb:
                    return new HealingOrbSkillModel(skillData);

                case SkillType.NatureBlessing:
                    return new NatureBlessingSkillModel(skillData);

                case SkillType.MultiShot:
                    return new MultiShotSkillModel(skillData);

                case SkillType.ChiwiStrike:
                    return new ChiwiStrikeSkillModel(skillData);

                case SkillType.Enrage:
                    return new EnrageSkillModel(skillData);

                case SkillType.CallOfTheMoon:
                    return new CallOfTheMoonSkillModel(skillData);

                case SkillType.FireBurst:
                    return new FireBurstSkillModel(skillData);

                case SkillType.HeadShot:
                    return new HeadShotSkillModel(skillData);

                case SkillType.SwampKingFirst:
                    return new SwampKingFirstSkillModel(skillData);

                case SkillType.SwampKingSecond:
                    return new SwampKingSecondSkillModel(skillData);

                case SkillType.FireBlast:
                    return new FireBlastSkillModel(skillData);

                case SkillType.ScorchedEarth:
                    return new ScorchedEarthSkillModel(skillData);

                case SkillType.SpawnDarkFlail:
                    return new SpawnDarkFlailSkillModel(skillData);

                case SkillType.ShurikenThrow:
                    return new ShurikenThrowSkillModel(skillData);

                case SkillType.SmokeBomb:
                    return new SmokeBombSkillModel(skillData);

                case SkillType.SpearThrow:
                    return new SpearThrowSkillModel(skillData);

                case SkillType.BraveHeart:
                    return new BraveHeartSkillModel(skillData);

                case SkillType.DarkBarrage:
                    return new DarkBarrageSkillModel(skillData);

                case SkillType.DarkBall:
                    return new DarkBallSkillModel(skillData);

                case SkillType.PowerShot:
                    return new PowerShotSkillModel(skillData);

                case SkillType.RainOfArrow:
                    return new RainOfArrowSkillModel(skillData);

                case SkillType.LightningBolt:
                    return new LightningBoltSkillModel(skillData);

                case SkillType.LightningHuge:
                    return new LightningHugeSkillModel(skillData);

                case SkillType.SwipeKick:
                    return new SwipeKickSkillModel(skillData);

                case SkillType.Slam:
                    return new SlamSkillModel(skillData);

                case SkillType.BurningStrike:
                    return new BurningStrikeSkillModel(skillData);

                case SkillType.FocusDefense:
                    return new FocusDefenseSkillModel(skillData);

                case SkillType.KitsuneBless:
                    return new KitsuneBlessSkillModel(skillData);

                case SkillType.KitsuneReincarnation:
                    return new KitsuneReincarnationSkillModel(skillData);

                case SkillType.PoisonousFlask:
                    return new PoisonousFlaskSkillModel(skillData);

                case SkillType.PandemicEnd:
                    return new PandemicEndSkillModel(skillData);

                case SkillType.BatSwarm:
                    return new BatSwarmSkillModel(skillData);

                case SkillType.BloodyNightmare:
                    return new BloodyNightmareSkillModel(skillData);

                case SkillType.HealingMagic:
                    return new HealingMagicSkillModel(skillData);

                case SkillType.MysticSpell:
                    return new MysticSpellSkillModel(skillData);

                case SkillType.GroundSlam:
                    return new GroundSlamSkillModel(skillData);

                case SkillType.BattleEndure:
                    return new BattleEndureSkillModel(skillData);

                case SkillType.MultiPaw:
                    return new MultiPawSkillModel(skillData);

                case SkillType.DeathClaws:
                    return new DeathClawsSkillModel(skillData);

                case SkillType.Swing:
                    return new SwingSkillModel(skillData);

                case SkillType.EarthShake:
                    return new EarthShakeSkillModel(skillData);

                case SkillType.Pray:
                    return new PraySkillModel(skillData);

                case SkillType.Purification:
                    return new PurificationSkillModel(skillData);

                case SkillType.HammerSlam:
                    return new HammerSlamSkillModel(skillData);

                case SkillType.Provoke:
                    return new ProvokeSkillModel(skillData);

                case SkillType.WeaknessStrike:
                    return new WeaknessStrikeSkillModel(skillData);

                case SkillType.FlurryOfKnives:
                    return new FlurryOfKnivesSkillModel(skillData);

                case SkillType.GoldenMimicFirst:
                    return new GoldenMimicFirstSkillModel(skillData);

                case SkillType.GoldenMimicSecond:
                    return new GoldenMimicSecondSkillModel(skillData);

                case SkillType.Smash:
                    return new SmashSkillModel(skillData);

                case SkillType.Collapse:
                    return new CollapseSkillModel(skillData);

                case SkillType.SnowBall:
                    return new SnowBallSkillModel(skillData);

                case SkillType.IceSplitter:
                    return new IceSplitterSkillModel(skillData);

                case SkillType.FireBall:
                    return new FireBallSkillModel(skillData);

                case SkillType.EndOfAnEra:
                    return new EndOfAnEraSkillModel(skillData);

                case SkillType.PunishmentLight:
                    return new PunishmentLightSkillModel(skillData);

                case SkillType.GoddessAura:
                    return new GoddessAuraSkillModel(skillData);

                case SkillType.Cleave:
                    return new CleaveSkillModel(skillData);

                case SkillType.Cyclone:
                    return new CycloneSkillModel(skillData);

                case SkillType.Madness:
                    return new MadnessSkillModel(skillData);

                case SkillType.Frenzy:
                    return new FrenzySkillModel(skillData);

                case SkillType.FanThrow:
                    return new FanThrowSkillModel(skillData);

                case SkillType.SpiningFan:
                    return new SpiningFanSkillModel(skillData);

                case SkillType.MultiStrike:
                    return new MultiStrikeSkillModel(skillData);

                case SkillType.Invincibility:
                    return new InvincibilitySkillModel(skillData);

                case SkillType.DarkFallen:
                    return new DarkFallenSkillModel(skillData);

                case SkillType.SoulRelease:
                    return new SoulReleaseSkillModel(skillData);

                case SkillType.StoneForm:
                    return new StoneFormSkillModel(skillData);

                case SkillType.RockThrow:
                    return new RockThrowSkillModel(skillData);

                case SkillType.DeathSlash:
                    return new DeathSlashSkillModel(skillData);

                case SkillType.Execute:
                    return new ExecuteSkillModel(skillData);

                case SkillType.Sweep:
                    return new SweepSkillModel(skillData);

                case SkillType.DeadLegion:
                    return new DeadLegionSkillModel(skillData);

                case SkillType.Kame:
                    return new KameSkillModel(skillData);

                case SkillType.FormChanging:
                    return new FormChangingSkillModel(skillData);

                case SkillType.GlaiveFlurry:
                    return new GlaiveFlurrySkillModel(skillData);

                case SkillType.BambooForest:
                    return new BambooForestSkillModel(skillData);

                case SkillType.WavingRage:
                    return new WavingRageSkillModel(skillData);

                case SkillType.Maelstrom:
                    return new MaelstromSkillModel(skillData);

                case SkillType.WildClaw:
                    return new WildClawSkillModel(skillData);

                case SkillType.WildSpirit:
                    return new WildSpiritSkillModel(skillData);

                case SkillType.PenetratingThrow:
                    return new PenetratingThrowSkillModel(skillData);

                case SkillType.SpearMaster:
                    return new SpearMasterSkillModel(skillData);

                case SkillType.EnhantedEncourage:
                    return new EnhantedEncourageSkillModel(skillData);

                case SkillType.EnhantedHeavyStrike:
                    return new EnhantedHeavyStrikeSkillModel(skillData);
                
                case SkillType.RocketBarrage:
                    return new RocketBarrageSkillModel(skillData);
                
                case SkillType.StatisPrison:
                    return new StatisPrisonSkillModel(skillData);

                case SkillType.Bonk:
                    return new BonkSkillModel(skillData);

                case SkillType.WukongCommand:
                    return new WukongCommandSkillModel(skillData);

            }

            return null;
        }

        #endregion Class Methods
    }
}