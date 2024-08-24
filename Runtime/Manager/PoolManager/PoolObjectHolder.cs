using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Manager.Pool
{
    /// <summary>
    /// A pool object holder holds a queue of pool objects, is reponsible for getting, returning, cleaning objects in the pool.
    /// </summary>
    public class PoolObjectHolder
    {
        #region Members

        private int _poolSize;
        private GameObject _objectPrefab;
        private Transform _parentTransform;
        private Queue<GameObject> _pooledObjectsQueue;

        #endregion Members

        #region Class Methods

        public PoolObjectHolder(GameObject objectPrefab, Transform parentTransform, int poolSize)
        {
            _poolSize = poolSize;
            _objectPrefab = objectPrefab;
            _parentTransform = parentTransform;
            _pooledObjectsQueue = new Queue<GameObject>();
            CreatePool();
        }

        public GameObject Get(bool isActive)
        {
            if (_pooledObjectsQueue.Count <= 0)
                CreatePool();

            var returnedPoolObject = _pooledObjectsQueue.Dequeue();
            returnedPoolObject.SetActive(isActive);
            return returnedPoolObject;
        }

        public void Return(GameObject gameObject)
        {
            gameObject.SetActive(false);
            _pooledObjectsQueue.Enqueue(gameObject);
        }

        private void CreatePool()
        {
            for (int i = 0; i < _poolSize; i++)
            {
                GameObject pooledObject = GameObject.Instantiate(_objectPrefab);
                pooledObject.transform.SetParent(_parentTransform);
                pooledObject.SetActive(false);
                _pooledObjectsQueue.Enqueue(pooledObject);
            }
        }

        #endregion Class Methods
    }
}