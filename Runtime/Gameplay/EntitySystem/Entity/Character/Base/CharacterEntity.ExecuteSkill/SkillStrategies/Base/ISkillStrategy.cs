using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public interface ISkillStrategy : IDisposable
    {
        #region Properties

        float CastRange { get; }

        #endregion Properties

        #region Interface Methods

        void Init(SkillModel skillModel, CharacterModel creatorModel, Transform creatorTransform);
        bool CheckCanUseSkill();
        bool CheckCanTriggerSkillWhenCompletedAttack();
        void GetUpdatedFromAttack();
        UniTask OperateSkillAsync();
        void Cancel();

        #endregion Interface Methods
    }
}