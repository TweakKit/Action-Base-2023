using System;
using DG.Tweening;
using UnityEngine;
using Runtime.Definition;
using Runtime.Utilities;

namespace Runtime.Common.UI
{
    public class SpriteRendererButton : MonoBehaviour
    {
        private static bool isClicking = false;
        #region Members

        [SerializeField] protected DOTweenAnimation _animation;
        public event Action OnButtonClicked;

        #endregion Members

        #region API Methods

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isClicking)
                    return;
                if (!InputUtility.IsPointerOverUIObject())
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero,
                        Layer.SPRITE_RENDERER_BUTTON_LAYER_MASK);
                    foreach (var hit in hits)
                    {
                        if (hit.collider != null && hit.collider.gameObject == gameObject)
                        {
                            if (this._animation != null)
                                _animation.DORestart();
                            isClicking = true;
                            OnButtonClicked?.Invoke();
                            break;
                        }
                    }
                }
            }
            else
            {
                isClicking = false;
            }
        }

        //private void OnMouseUpAsButton()
        //{
        //    if (this._animation != null)
        //        _animation.DORestart();
        //    OnButtonClicked?.Invoke();
        //}

        #endregion API Methods

        #region Class Methods

        public void AddListener(Action onClickAction)
            => OnButtonClicked += onClickAction;

        public void RemoveListener(Action onClickAction)
            => OnButtonClicked -= onClickAction;

        public void RemoveAllListeners()
            => OnButtonClicked = null;


        #endregion Class Methods
    }
}