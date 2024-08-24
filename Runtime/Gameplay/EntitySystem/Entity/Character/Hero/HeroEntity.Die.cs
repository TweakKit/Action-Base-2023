using Runtime.Message;
using Runtime.Gameplay.Manager;
using Runtime.Audio;
using Cysharp.Threading.Tasks;
using Runtime.Manager.Data;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class HeroEntity : CharacterEntity<HeroModel>
    {
        #region Class Methods

        protected override void OnDeathOnDie(DamageSource damageSource)
        {
            AudioController.Instance?.PlaySoundEffectAsync(AudioConstants.GENERAL_DIE, this.GetCancellationTokenOnDestroy()).Forget();
            base.OnDeathOnDie(damageSource);
            var respawnDelay = ownerModel.RespawnDelay;
            if (DataManager.Transitioned.IsOnPremiumCamp && DataManager.Transitioned.IsOnCamping)
            {
                respawnDelay -= DataManager.Config.GetDescreaseRespawnTimePremiumCamp();
                if (respawnDelay < 0) respawnDelay = 0;
            }
            Messenger.Publish(new HeroDiedMessage(ownerModel, transform, respawnDelay));
            EntitiesManager.Instance.RemoveEntity(gameObject);
        }

        #endregion Class Methdos
    }
}