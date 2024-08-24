using System;

namespace Runtime.Gameplay.EntitySystem
{
    /// <summary>
    /// This interface is the input for which type of "action player" will take care of when to trigger the attack related behaviors.<br/>
    /// Examples of those inputs such as:<br/>
    ///     + Sprite Animation Action Player: Play actions by sprite animation.<br/>
    ///     + Clip Animation Action Player: Play actions by clip animation (Unity animation).<br/>
    ///     + Timing Action Player: Play actions by manual timing.<br/>
    ///     + ...
    /// </summary>
    public interface ICharacterWeaponActionPlayer
    {
        #region Interface Methods

        public void Init();
        public void Play(CharacterWeaponPlayedData playedData);
        public float GetAnimationTime(CharacterWeaponAnimationType characterWeaponAnimationType);
        public void SetNewAnimationName(CharacterWeaponAnimationType characterWeaponAnimationType, string newMappingName);

        #endregion Interface Methods
    }

    public struct CharacterWeaponPlayedData
    {
        #region Members

        public CharacterWeaponAnimationType animationType;
        public float speedMultiplier;
        public Action operatedPointTriggeredCallbackAction;
        public Action endActionCallbackAction;

        #endregion Members

        #region Struct Methods

        public CharacterWeaponPlayedData(CharacterWeaponAnimationType animationType, float speedMultiplier, Action operatedPointTriggeredCallbackAction, Action endActionCallbackAction)
        {
            this.animationType = animationType;
            this.speedMultiplier = speedMultiplier;
            this.operatedPointTriggeredCallbackAction = operatedPointTriggeredCallbackAction;
            this.endActionCallbackAction = endActionCallbackAction;
        }

        #endregion Struct Methods
    }
}