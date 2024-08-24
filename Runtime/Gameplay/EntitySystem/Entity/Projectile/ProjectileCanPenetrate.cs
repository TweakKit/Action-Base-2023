namespace Runtime.Gameplay.EntitySystem
{
    public class ProjectileCanPenetrate : Projectile
    {
        public void SetSingleTarget(bool isSingleTarget)
        {
            this.isSingleTarget = isSingleTarget;
        }
    }
}