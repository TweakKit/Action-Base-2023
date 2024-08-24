using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Navigation
{
    public struct RectangleFormationShape : IFormationShape
    {
        #region Members

        private int _columnCount;
        private float _spacing;
        private bool _centerUnits;
        private bool _pivotInMiddle;

        #endregion Members

        #region Struct Methods

        public RectangleFormationShape(int columnCount, float spacing, bool centerUnits = true, bool pivotInMiddle = false)
        {
            _columnCount = columnCount;
            _spacing = spacing;
            _centerUnits = centerUnits;
            _pivotInMiddle = pivotInMiddle;
        }

        public List<Vector2> GetPositions(int unitCount)
        {
            List<Vector2> unitPositions = new List<Vector2>();
            var unitsPerRow = Mathf.Min(_columnCount, unitCount);
            float offsetX = (unitsPerRow - 1) * _spacing / 2f;

            if (unitsPerRow == 0)
                return new List<Vector2>();

            float rowCount = unitCount / _columnCount + (unitCount % _columnCount > 0 ? 1 : 0);
            float x, y, column;
            int firstIndexInRow;

            for (int row = 0; unitPositions.Count < unitCount; row++)
            {
                firstIndexInRow = row * _columnCount;
                if (_centerUnits && row != 0 && firstIndexInRow + _columnCount > unitCount)
                {
                    var emptySlots = firstIndexInRow + _columnCount - unitCount;
                    offsetX -= emptySlots / 2f * _spacing;
                }

                for (column = 0; column < _columnCount; column++)
                {
                    if (firstIndexInRow + column < unitCount)
                    {
                        x = column * _spacing - offsetX;
                        y = row * _spacing;
                        var newPosition = new Vector2(x, -y);
                        unitPositions.Add(newPosition);
                    }
                    else
                    {
                        if (_pivotInMiddle)
                            ApplyFormationCentering(ref unitPositions, rowCount, _spacing);
                        return unitPositions;
                    }
                }
            }

            if (_pivotInMiddle)
                ApplyFormationCentering(ref unitPositions, rowCount, _spacing);
            return unitPositions;
        }

        private static void ApplyFormationCentering(ref List<Vector2> positions, float rowCount, float rowSpacing)
        {
            float offsetY = Mathf.Max(0, (rowCount - 1) * rowSpacing / 2);
            for (int i = 0; i < positions.Count; i++)
            {
                var pos = positions[i];
                pos.y += offsetY;
                positions[i] = pos;
            }
        }

        #endregion Struct Methods
    }
}