using Runtime.Message;
using Runtime.Definition;
using Runtime.Manager.Pool;
using Runtime.Manager.Proximity;
using Runtime.Gameplay.Manager;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EnemyEntity : CharacterEntity<EnemyModel>
    {
        #region Class Methods

        protected override void OnDeathOnDie(DamageSource damageSource)
        {
            base.OnDeathOnDie(damageSource);
            Messenger.Publish(new EnemyDiedMessage(ownerModel, ownerModel.RespawnDelay));
            EntitiesManager.Instance.RemoveEntity(gameObject);
            ProximityManager.Instance.Remove(this);
            SpawnDestroyedVFX().Forget();
        }

        private async UniTaskVoid SpawnDestroyedVFX()
        {
            var destroyedVFX = await PoolManager.Instance.Get(Constant.GetEntityExplosionEffectName(ownerModel.EntityType),
                                                              this.GetCancellationTokenOnDestroy());
            var scale = (ownerModel.IsElite)?  Vector3.one * Constant.ELITE_RATIO_SCALE_EXPLODE_VFX : 
                                               Vector3.one * Constant.ORIGINAL_RATIO_SCALE_EXPLODE_VFX;
            destroyedVFX.transform.localScale = scale;
            destroyedVFX.transform.position = ownerModel.Position;
        }

        #endregion Class Methdos
    }
}