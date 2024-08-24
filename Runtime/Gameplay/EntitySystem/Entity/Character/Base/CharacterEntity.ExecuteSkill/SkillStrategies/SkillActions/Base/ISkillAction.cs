using System.Threading;
using UnityEngine;
using Runtime.Definition;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public interface ISkillAction
    {
        #region Properties

        public SkillActionPhase SkillActionPhase { get; }
        public bool HasOperatedCastPoint { get; }

        #endregion Properties

        #region Interface Methods

        void Init(CharacterModel creatorModel, Transform creatorTransform, SkillType skillType,
                  SkillTargetType targetType, SkillActionPhase skillActionPhase, float castRange);
        void PreExecuteAction();
        UniTask ExecuteActionAsync(SkillActionTransitionedData skillActionTransitionedData, CancellationToken cancellationToken);
        void Cancel();

        #endregion Interface Methods
    }
}