using UnityEngine;

namespace Runtime.Utilities
{
    public static class ScreenUtility
    {
        #region Class Methods

        public static bool IsPositionOffScreen(Vector2 position)
        {
            var screenPosition = Camera.main.WorldToViewportPoint(position);
            return screenPosition.x <= 0 || screenPosition.x >= 1 || screenPosition.y <= 0 || screenPosition.y >= 1;
        }

        #endregion Class Methods
    }
}