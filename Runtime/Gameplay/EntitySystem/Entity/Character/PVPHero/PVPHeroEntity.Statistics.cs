using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class PVPHeroEntity : HeroEntity
    {
        #region Class Methods
        
        private partial void PartialValidateStatistics() { }
        private partial void PartialDisposeStatistics() { }
    
        private partial void PartialInitializeStatistics()
        {
            if (ownerModel is PVPHeroModel)
                ownerModel.HealthChangedEvent += OnHealthChangedStatistics;
        }
        
        protected virtual void OnHealthChangedStatistics(float deltaHp, DamageSource damageSource)
        {
            if (deltaHp > 0)
            {
                if (ownerModel is PVPHeroModel ownerPVPHeroModel)
                    ownerPVPHeroModel.AddDamageHealedStatistics(deltaHp);
            }
            else if(deltaHp < 0)
            {
                if (damageSource != DamageSource.None)
                {
                    if (ownerModel is PVPHeroModel ownerPVPHeroModel)
                        ownerPVPHeroModel.AddDamageReceivedStatistics(-deltaHp);
                }
            }
        }
        
        #endregion Class Methods
    }
}