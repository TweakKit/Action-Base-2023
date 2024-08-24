namespace Runtime.Gameplay.EntitySystem
{
    public partial class HarmlessEnemyEntity : EnemyEntity
    {
        #region Class Methods

        protected override void UpdateClosestTarget()
            => ownerModel.UpdateTargetedTarget(null);

        #endregion Class Methods
    }
}