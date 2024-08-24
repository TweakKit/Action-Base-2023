using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Runtime.Common.Singleton;
using Cysharp.Threading.Tasks;

namespace Runtime.Manager.Pool
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        #region Members

        private static readonly int s_initialPoolSize = 1;
        private Dictionary<string, PoolObjectHolder> _poolObjectHoldersDictionary;

        #endregion Members

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            _poolObjectHoldersDictionary = new Dictionary<string, PoolObjectHolder>();
        }

        #endregion API Methods

        #region Class Methods

        public async UniTask<GameObject> Get(string prefabId, CancellationToken cancellationToken, bool isActive = true)
        {
            if (!_poolObjectHoldersDictionary.TryGetValue(prefabId, out var poolObjectHolder))
            {
                var objectPrefab = await Addressables.LoadAssetAsync<GameObject>(prefabId).WithCancellation(cancellationToken);
                poolObjectHolder = new PoolObjectHolder(objectPrefab, this.transform, s_initialPoolSize);
                _poolObjectHoldersDictionary[prefabId] = poolObjectHolder;
            }
            var pooledObject = poolObjectHolder.Get(isActive);
            pooledObject.name = prefabId;
            pooledObject.transform.SetParent(null);
            pooledObject.SetActive(isActive);
            return pooledObject;
        }

        public void Remove(GameObject gameObject)
        {
            var pooledObjectId = gameObject.name;
            if (_poolObjectHoldersDictionary.ContainsKey(pooledObjectId))
            {
                gameObject.transform.SetParent(transform);
                _poolObjectHoldersDictionary[pooledObjectId].Return(gameObject);
            }
            else gameObject.SetActive(false);
        }

        #endregion Class Methods
    }
}