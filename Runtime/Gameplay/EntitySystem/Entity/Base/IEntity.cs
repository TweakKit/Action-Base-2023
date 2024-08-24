using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntity
    {
        #region Properties

        uint EntityUId { get; }
        bool IsActive { get; }
        Vector3 Position { get; }

        #endregion Properties

        #region Interface Methods

        void Build(EntityModel model, Vector3 position);
        void SetActive(bool isActive);
        void SetAnimationUnscaled(bool isUnscaled);

        #endregion Interface Methods
    }
}