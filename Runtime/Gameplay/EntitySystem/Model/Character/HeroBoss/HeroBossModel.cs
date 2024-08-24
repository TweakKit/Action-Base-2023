namespace Runtime.Gameplay.EntitySystem
{
    public class HeroBossModel : BossModel
    {
        #region Members

        private float _hpScaleFactor;

        #endregion Members

        #region Properties

        public float HpScaleFactor => _hpScaleFactor;
        public override bool IsHeroBoss => true;

        #endregion Properties

        #region Class Methods

        public HeroBossModel(uint bossUId, string bossId, float hpScaleFactor, bool isImmortal, BossModelData bossModelData,
                             int zoneLevel, float dropEquipmentRate, bool markRepspawnable)
            : base(bossUId, bossId, isImmortal, bossModelData, zoneLevel, dropEquipmentRate, markRepspawnable)
            => _hpScaleFactor = hpScaleFactor;

        #endregion Class Methods
    }
}