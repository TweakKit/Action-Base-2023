using System;
using System.Linq;
using UnityEngine;
using Runtime.Animation;
using Sirenix.OdinInspector;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public class CharacterWeaponSpriteAnimation
    {
        #region Members

        public CharacterWeaponAnimationType weaponAnimationType;
        public string mappedAnimationName;
        public bool hasEventTriggeredAtAFrame;
        [ShowIf(nameof(hasEventTriggeredAtAFrame), true)]
        public int eventTriggeredFrame;

        #endregion Members
    }

    public class CharacterWeaponSpriteAnimationActionPlayer : MonoBehaviour, ICharacterWeaponActionPlayer
    {
        #region Members

        [SerializeField]
        private ObjectSpriteAnimator _spriteAnimator;
        [SerializeField]
        private CharacterWeaponSpriteAnimation[] _spriteAnimations;

        #endregion Members

        #region Class Methods

        public void Init() { }

        public float GetAnimationTime(CharacterWeaponAnimationType characterWeaponAnimationType)
        {
            var spriteAnimation = _spriteAnimations.FirstOrDefault(x => x.weaponAnimationType == characterWeaponAnimationType);
            if (spriteAnimation != null && _spriteAnimator != null)
            {
                return _spriteAnimator.GetAnimationTime(spriteAnimation.mappedAnimationName);
            }
            return 0;
        }

        public void Play(CharacterWeaponPlayedData playedData)
        {
            var spriteAnimation = _spriteAnimations.FirstOrDefault(x => x.weaponAnimationType == playedData.animationType);
            if (spriteAnimation != null)
            {
                if (spriteAnimation.hasEventTriggeredAtAFrame)
                {
                    _spriteAnimator.Play(spriteAnimation.mappedAnimationName,
                                         animateSpeedMultiplier: playedData.speedMultiplier,
                                         playOneShot: true,
                                         eventTriggeredCallbackAction: playedData.operatedPointTriggeredCallbackAction,
                                         eventTriggeredFrame: spriteAnimation.eventTriggeredFrame);
                }
                else
                {
                    _spriteAnimator.Play(spriteAnimation.mappedAnimationName,
                                         playOneShot: true);
                }

                _spriteAnimator.AnimationStoppedAction = playedData.endActionCallbackAction;
            }
        }
        
        public void SetNewAnimationName(CharacterWeaponAnimationType characterWeaponAnimationType, string newMappingName)
        {
            var stateSpriteAnimation = _spriteAnimations.FirstOrDefault(s => s.weaponAnimationType == characterWeaponAnimationType);
            if (stateSpriteAnimation != null)
            {
                stateSpriteAnimation.mappedAnimationName = newMappingName;
            }
        }

        #endregion Class Methods
    }
}