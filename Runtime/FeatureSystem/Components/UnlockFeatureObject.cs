using UnityEngine;

namespace Runtime.FeatureSystem
{
    public class UnlockFeatureObject : UnlockedFeature
    {
        #region Members

        [SerializeField]
        private GameObject[] _disableObjectsWhenLock;

        #endregion Members

        public override void UpdateStatus(bool isUnlocked)
        {
            foreach (var disableObjectWhenLock in _disableObjectsWhenLock)
                disableObjectWhenLock.SetActive(isUnlocked);
        }
    }
}