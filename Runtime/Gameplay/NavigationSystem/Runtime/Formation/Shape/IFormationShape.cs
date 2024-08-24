using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Navigation
{
    public interface IFormationShape
    {
        #region Interface Methods

        List<Vector2> GetPositions(int unitCount);

        #endregion Interface Methods
    }
}