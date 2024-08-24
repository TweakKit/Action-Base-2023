using Runtime.Message;
using Runtime.Definition;
using Runtime.Manager.Pool;
using Runtime.Manager.Proximity;
using Runtime.Gameplay.Manager;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class ObjectEntity : Entity<ObjectModel>
    {
        #region Class Methods

        private partial void PartialValidateDestroy() { }
        private partial void PartialDisposeDestroy() { }

        private partial void PartialInitializeDestroy()
            => ownerModel.DestroyEvent += OnDestroyed;

        private void OnDestroyed()
        {
            SpawnDestroyedVFX().Forget();
            Messenger.Publish(new ObjectDestroyedMessage(ownerModel, ownerModel.RespawnDelay));
            EntitiesManager.Instance.RemoveEntity(gameObject);
            ProximityManager.Instance.Remove(this);
        }

        private async UniTaskVoid SpawnDestroyedVFX()
        {
            var destroyedVFX = await PoolManager.Instance.Get(Constant.GetEntityExplosionEffectName(ownerModel.EntityType),
                                                              this.GetCancellationTokenOnDestroy());
            destroyedVFX.transform.position = ownerModel.Position;
        }

        #endregion Class Methods
    }
}