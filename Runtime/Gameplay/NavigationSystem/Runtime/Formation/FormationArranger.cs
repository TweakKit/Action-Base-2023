using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Navigation
{
    /// <summary>
    /// This class's responsible for providing unit positions in formation on
    /// a target position facing the respective angle.</summary>
    public static class FormationArranger
    {
        #region Class Methods

        /// <summary>
        /// Return aligned units formation positions that are facing the passed angle.
        /// </summary>
        /// <param name="unitCount">Number of units in formation.</param>
        /// <param name="formationShape">Formation shape that units will position in.</param>
        /// <param name="targetPosition">Position of the formation.</param>
        /// <param name="targetAngle">Facing angle for the formation.</param>
        /// <returns>Return aligned positions of the units in formation.</returns>
        public static List<Vector2> GetAlignedPositions(int unitCount, IFormationShape formationShape)
        {
            var positions = formationShape.GetPositions(unitCount);
            return positions;
        }

        /// <summary>
        /// Finds new positions for the passed positions and the formation.
        /// If distance from current positions center is less than rotation
        /// threshold, units formation will not be rotated around the target.
        /// New rotation angle is calculated from center position of all current
        /// positions and the target positions.
        /// </summary>
        /// <param name="currentPositions">Current unit positions.</param>
        /// <param name="formationShape">Formation shape used on units.</param>
        /// <param name="targetPosition">Position to where the units will be moved.</param>
        /// <param name="rotationThreshold">Threshold used to specify when the
        /// unit formation should be rotated around target position (pivot).</param>
        /// <returns>Returns list of the new unit positions and their new facing angle</returns>
        public static UnitsFormationData GetPositions(List<Vector3> currentPositions, IFormationShape formationShape, Vector3 targetPosition, float rotationThreshold = 4.0f)
        {
            if (currentPositions.Count == 0)
            {
                Debug.LogWarning("Cannot generate formation for an empty game object list.");
                return new UnitsFormationData(new List<Vector2>(), 0.0f);
            }

            // Get sum of all positions in order to get center of the objects.
            Vector3 sumPosition = new Vector3();
            foreach (Vector3 position in currentPositions)
                sumPosition += position;

            var centerPosition = sumPosition / currentPositions.Count;
            var direction = targetPosition - centerPosition;
            float angle = 0;

            // Only if direction change is significant, it should rotate units formation as well.
            if (direction.magnitude > rotationThreshold)
                angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            var formationPositions = GetAlignedPositions(currentPositions.Count, formationShape);
            return new UnitsFormationData(formationPositions, angle);
        }

        private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle)
            => Quaternion.Euler(angle) * (point - pivot) + pivot;

        #endregion Class Methods
    }

    /// <summary>
    /// Data structure that represents the units new formation positions and angles.
    /// </summary>
    public struct UnitsFormationData
    {
        #region Members

        public List<Vector2> unitPositions;
        public float facingAngle;

        #endregion Members

        #region Struct Methods

        public UnitsFormationData(List<Vector2> unitPositions, float facingAngle)
        {
            this.unitPositions = unitPositions;
            this.facingAngle = facingAngle;
        }

        #endregion Struct Methods
    }
}