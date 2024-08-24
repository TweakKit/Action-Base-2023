using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IAttackStrategy : IDisposable
    {
        #region Interface Methods

        void Init(CharacterModel ownerCharacterModel, Transform ownerCharacterTransform);
        bool CheckCanAttack();
        UniTask OperateAttackAsync();
        bool Cancel();

        #endregion Interface Methods
    }
}