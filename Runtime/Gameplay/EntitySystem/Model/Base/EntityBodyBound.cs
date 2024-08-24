namespace Runtime.Gameplay.EntitySystem
{
    public struct EntityBodyBound
    {
        #region Members

        public float width;
        public float height;
        public float verticalOffset;

        #endregion Members

        #region Properties

        public float Width => width;
        public float Height => height;
        public float VerticalOffset => verticalOffset;

        #endregion Properties

        #region Struct Methods

        public EntityBodyBound(float width, float height, float verticalOffset)
        {
            this.width = width;
            this.height = height;
            this.verticalOffset = verticalOffset;
        }

        #endregion Struct Methods
    }
}