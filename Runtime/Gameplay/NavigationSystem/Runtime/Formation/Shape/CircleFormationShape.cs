using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Navigation
{
    public struct CircleFormationShape : IFormationShape
    {
        #region Members

        private float _spacing;
        private float _minUnitsDistance;

        #endregion Members

        #region Struct Methods

        public CircleFormationShape(float spacing, float minUnitsDistance)
        {
            _spacing = spacing;
            _minUnitsDistance = minUnitsDistance;
        }

        public List<Vector2> GetPositions(int unitsCount)
        {
            if (unitsCount <= 1)
                unitsCount = 2;

            var unitPositions = new List<Vector2>();
            float angle = 0.0f;
            var increasedAngle = 360.0f / unitsCount;
            var minRadius = Mathf.Max(_spacing * 0.5f, _minUnitsDistance);
            var maxRadius = Mathf.Max(_spacing, _minUnitsDistance);
            for (int i = 0; i < unitsCount; i++)
            {
                var randomRadius = Random.Range(minRadius, maxRadius);
                var x = Mathf.Cos(Mathf.Deg2Rad * angle) * randomRadius;
                var y = Mathf.Sin(Mathf.Deg2Rad * angle) * randomRadius;
                unitPositions.Add(new Vector3(x, y));
                angle += increasedAngle;
            }

            return unitPositions;
        }

        #endregion Struct Methods
    }
}