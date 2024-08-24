using UnityEngine;

namespace Runtime.Manager.Pool
{
    public class ReturnPoolAfterDuration : ReturnPool
    {
        #region Members

        [SerializeField]
        [Min(0.001f)]
        protected float duration = 1.0f;

        #endregion Members

        #region Properties

        protected override float ReturnDelayTime => duration;

        #endregion Properties
    }
}