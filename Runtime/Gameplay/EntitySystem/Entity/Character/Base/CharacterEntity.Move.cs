using UnityEngine;
using Runtime.Navigation;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract partial class CharacterEntity<T> : Entity<T> where T : CharacterModel
    {
        #region Members

        protected float moveSpeed;
        protected CustomNavigationAgent navigationAgent;

        #endregion Members

        #region Class Methods

        protected virtual partial void PartialValidateMove()
        {
            navigationAgent = transform.GetComponent<CustomNavigationAgent>();
            if (navigationAgent == null)
            {
                Debug.LogError("Require a Navigation Agent component!");
                return;
            }
        }

        protected virtual partial void PartialInitializeMove()
        {
            navigationAgent = transform.GetComponent<CustomNavigationAgent>();
            navigationAgent.SetOwnerModel(ownerModel);
            moveSpeed = ownerModel.GetTotalStatValue(StatType.MoveSpeed);
            navigationAgent.maxSpeed = moveSpeed;
            ownerModel.MovePositionUpdatedEvent += OnMovePositionUpdatedOnMove;
            ownerModel.StatChangedEvent += OnStatChangedOnMove;
        }

        protected virtual partial void PartialDisposeMove()
            => navigationAgent.Stop();

        protected virtual partial void PartialUpdateMove() { }

        protected virtual void OnStatChangedOnMove(StatType statType, float updatedValue)
        {
            if (statType == StatType.MoveSpeed)
            {
                moveSpeed = updatedValue;
                navigationAgent.maxSpeed = moveSpeed;
            }
        }

        private void OnMovePositionUpdatedOnMove()
        {
            transform.position = ownerModel.MovePosition;
            ownerModel.Position = transform.position;
            navigationAgent.Stop();
        }

        #endregion Class Methods
    }
}