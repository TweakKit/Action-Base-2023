using UnityEngine;
using Runtime.Definition;

namespace Runtime.Animation
{
    /// <summary>
    /// This controls the animation that uses sprites.
    /// </summary>
    public class ObjectSpriteAnimator : SpriteAnimator
    {
        #region Members

        [SerializeField]
        protected SpriteRenderer spriteRenderer;

        #endregion Members

        #region Class Methods

        public override void TintColor(Color color)
            => spriteRenderer.material.SetColor(Constant.HIT_MATERIAL_COLOR_PROPERTY, color);

        public override void ChangeFrame(int frameIndex)
        {
            var sprite = currentAnimation.GetFrame(frameIndex);
            if (sprite != null)
                spriteRenderer.sprite = sprite;
        }

        public override void ClearRenderer()
            => spriteRenderer.sprite = null;

        #endregion Class Methods
    }
}