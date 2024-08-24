using UnityEngine;
using Runtime.Common.UI;
using Runtime.Manager.Toast;

namespace Runtime.FeatureSystem
{
    [RequireComponent(typeof(UnlockedFeature))]
    public class FeatureButton : PerfectButton, IUnlockable
    {
        #region Members

        protected UnlockedFeature unlockFeature;

        #endregion Members

        #region Properties

        public bool IsUnlocked => unlockFeature.IsUnlocked;
        public string RequiredUnlockDescription => unlockFeature.UnlockDescription;

        #endregion Properties

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            unlockFeature = gameObject.GetComponentInChildren<UnlockedFeature>();
        }

        #endregion API Methods

        #region Class Methods

        protected override bool Press()
        {
            if (!IsUnlocked)
            {
                ToastManager.Instance.Show(RequiredUnlockDescription);
                return false;
            }
            else return base.Press();
        }

        #endregion Class Methods
    }
}