using UnityEngine;

namespace Runtime.Manager.Input
{
    public interface IVector3Input
    {
        #region Properties

        Vector3 Input { get; }

        #endregion Properties

        #region Interface Methods

        void Reset();
        void Disable();
        void Enable();

        #endregion Interface Methods
    }
}