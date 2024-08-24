using Runtime.Manager.Data;

namespace Runtime.FeatureSystem
{
    public struct PVPFeatureUnlockDefinition : IFeatureUnlockDefinition
    {
        #region Members

        private const int REQUIRED_PLAYER_LEVEL = 40;

        #endregion Members

        #region Struct Methods

        public bool IsUnlocked()
            => DataManager.Local.CheckPlayerLevel(REQUIRED_PLAYER_LEVEL);

        #endregion Struct Methods
    }
}