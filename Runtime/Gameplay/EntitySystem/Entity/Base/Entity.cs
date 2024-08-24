using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class Entity<T> : Disposable, IEntity where T : EntityModel
    {
        #region Members

        protected T ownerModel;

        #endregion Members

        #region Properties

        public uint EntityUId { get; private set; }
        public bool IsActive => ownerModel.IsActive;
        public Vector3 Position => ownerModel.Position;

        #endregion Properties

        #region API Methods

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (Application.isPlaying)
                return;

            ExecuteValidate();
        }
#endif

        #endregion API Methods

        #region Class Methods

        public virtual void Build(EntityModel model, Vector3 position)
        {
            ownerModel = model as T;
            EntityUId = model.EntityUId;
            HasDisposed = false;
            SetUpPosition(position);
            SetUpScale();
            SetUpBound();
            ExecuteInitialize();
        }

        public virtual void SetActive(bool isActive)
        {
            ownerModel.SetActive(isActive);
            gameObject.SetActive(isActive);
        }

        public virtual void SetAnimationUnscaled(bool isUnscaled) { }

        public override void Dispose()
        {
            if (!HasDisposed)
            {
                HasDisposed = true;
                ExecuteDispose();
            }
        }

        protected virtual void SetUpPosition(Vector3 position)
        {
            transform.position = position;
            ownerModel.Position = position;
            ownerModel.OriginalPosition = position;
        }

        protected virtual void SetUpScale()
            => transform.localScale = Vector3.one;

        protected virtual void SetUpBound()
        {
            var localScale = transform.localScale;
            var collider = gameObject.GetComponent<Collider2D>();
            if (collider != null)
            {
                if (collider.GetType() == typeof(BoxCollider2D))
                {
                    var boxCollider = collider as BoxCollider2D;
                    ownerModel.BodyBound = new EntityBodyBound(boxCollider.size.x * localScale.x,
                                                               boxCollider.size.y * localScale.y,
                                                               boxCollider.offset.y * localScale.y);
                }
                else if (collider.GetType() == typeof(CircleCollider2D))
                {
                    var circleCollider = collider as CircleCollider2D;
                    ownerModel.BodyBound = new EntityBodyBound(circleCollider.radius * 2 * localScale.x,
                                                               circleCollider.radius * 2 * localScale.y,
                                                               circleCollider.offset.y * localScale.y);
                }
                else if (collider.GetType() == typeof(CapsuleCollider2D))
                {
                    var capsuleCollider = collider as CapsuleCollider2D;
                    ownerModel.BodyBound = new EntityBodyBound(capsuleCollider.size.x * localScale.x,
                                                               capsuleCollider.size.y * localScale.y,
                                                               capsuleCollider.offset.y * localScale.y);
                }
                else ownerModel.BodyBound = new EntityBodyBound();
            }
            else ownerModel.BodyBound = new EntityBodyBound();
        }

        protected virtual void ExecuteValidate() { }
        protected virtual void ExecuteInitialize() { }
        protected virtual void ExecuteDispose() { }

        #endregion Class Methods
    }
}