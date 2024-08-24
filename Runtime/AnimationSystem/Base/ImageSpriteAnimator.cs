using UnityEngine;
using Runtime.Definition;
using UnityEngine.UI;
using System;

namespace Runtime.Animation
{
    /// <summary>
    /// This controls the animation that uses Image.
    /// </summary>
    public class ImageSpriteAnimator : SpriteAnimator
    {
        #region Members

        [SerializeField] private Image _image;

        #endregion Members

        #region Methods

        public override void TintColor(Color color)
            => _image.material.SetColor(Constant.HIT_MATERIAL_COLOR_PROPERTY, color);

        public override void ChangeFrame(int frameIndex)
        {
            var sprite = currentAnimation.GetFrame(frameIndex);
            if (sprite != null)
            {
                _image.sprite = sprite;
                _image.rectTransform.sizeDelta = 100 * sprite.rect.size / sprite.pixelsPerUnit;
            }
        }

        public override void ClearRenderer()
            => _image.sprite = null;


        public override void Play(string animation, float animateSpeedMultiplier = 1.0f, bool playOneShot = false,
            Action eventTriggeredCallbackAction = null, int eventTriggeredFrame = -1, bool playBackwards = false,
            LoopType loopType = LoopType.Repeat)
        {
            currentAnimation = GetAnimation(animation);
            base.Play(animation, animateSpeedMultiplier, playOneShot, eventTriggeredCallbackAction, eventTriggeredFrame,
                playBackwards, loopType);
        }

        #endregion Methods
    }
}