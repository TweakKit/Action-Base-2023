using UnityEngine;

namespace Runtime.UI
{
    public class InfoBoxData
    {
        #region Members

        private string _title;
        private string _description;
        private bool _isPivotTop;
        private bool _isPivotRight;
        private Vector2 _position;

        #endregion Members

        #region Properties

        public string Title => _title;
        public string Description => _description;
        public bool IsPivotTop => _isPivotTop;
        public bool IsPivotRight => _isPivotRight;
        public Vector2 Position => _position;

        #endregion Properties

        #region Struct Methods

        public InfoBoxData(string title, string description)
        {
            _title = title;
            _description = description;
        }

        public void SetPivot(bool isPivotTop, bool isPivotRight)
        {
            _isPivotTop = isPivotTop;
            _isPivotRight = isPivotRight;
        }

        public void SetPosition(Vector2 position)
            => _position = position;

        #endregion Struct Methods
    }
}