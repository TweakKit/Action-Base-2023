using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IProjectile
    {
        #region Properties

        CharacterModel CreatorModel { get; }

        #endregion Properties

        #region Interface Methods

        void Build(CharacterModel creatorModel, Vector3 position);

        #endregion Interface Methods
    }
}