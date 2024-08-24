using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Foundation;
using UnityScreenNavigator.Runtime.Foundation.Animation;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Modals
{
    public class ModalBackdrop : MonoBehaviour
    {
        [SerializeField] private ModalBackdropTransitionAnimationContainer _animationContainer;
        [SerializeField] private float _bonusScale;
        protected CanvasGroup canvasGroup;
        protected RectTransform parentTransform;
        protected RectTransform rectTransform;
        protected Image image;
        protected float originalAlpha;
        protected Modal ownerModal;

        public ModalBackdropTransitionAnimationContainer AnimationContainer => _animationContainer;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
            canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            if (!TryGetComponent<Image>(out var image))
                image = gameObject.AddComponent<Image>();

            this.image = GetComponent<Image>();
            this.originalAlpha = this.image ? this.image.color.a : 1f;
            this.image.color = Color.clear;
        }

        protected virtual void PopModal()
        {
            var modalContainer = ownerModal.transform.parent.GetComponent<ModalContainer>();
            if (modalContainer.IsInTransition)
                return;

            modalContainer.Pop(true);
        }

        public void Setup(RectTransform parentTransform, float? alpha)
        {
            SetAlpha(alpha);

            this.parentTransform = parentTransform;
            rectTransform.FillParentAndScale(parentTransform, _bonusScale);
            canvasGroup.interactable = true;

            gameObject.SetActive(false);
        }

        public void SetOwnerModal(Modal ownerModal)
        {
            this.ownerModal = ownerModal;

            if (!TryGetComponent<Button>(out var button))
            {
                button = gameObject.AddComponent<Button>();
                button.transition = Selectable.Transition.None;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnClickBackdrop);
            }
        }

        public void DestroyBeforeSceneChanged()
        {
            SetAlpha(1.0f);
            Destroy(gameObject);
        }

        private void OnClickBackdrop()
        {
            if (ownerModal.CanCloseAsClickOnBackdrop)
                PopModal();
        }

        private void SetAlpha(float? value)
        {
            var image = this.image;

            if (!image)
            {
                return;
            }

            var alpha = originalAlpha;

            if (value.HasValue)
            {
                alpha = value.Value;
            }

            var color = image.color;
            color.a = alpha;
            image.color = color;
        }

        internal AsyncProcessHandle Enter(bool playAnimation)
        {
            return CoroutineManager.Run<ModalBackdrop>(EnterRoutine(playAnimation));
        }

        private IEnumerator EnterRoutine(bool playAnimation)
        {
            gameObject.SetActive(true);
            rectTransform.FillParentAndScale(parentTransform, _bonusScale);
            canvasGroup.alpha = 1;

            if (playAnimation)
            {
                var anim = _animationContainer.GetAnimation(true);
                if (anim == null)
                {
                    anim = UnityScreenNavigatorSettings.Instance.ModalBackdropEnterAnimation;
                }

                anim.Setup(rectTransform);
                yield return CoroutineManager.Run<ModalBackdrop>(anim.CreatePlayRoutine());
            }

            rectTransform.FillParentAndScale(parentTransform, _bonusScale);
        }

        internal AsyncProcessHandle Exit(bool playAnimation)
        {
            return CoroutineManager.Run<ModalBackdrop>(ExitRoutine(playAnimation));
        }

        private IEnumerator ExitRoutine(bool playAnimation)
        {
            gameObject.SetActive(true);
            rectTransform.FillParentAndScale(parentTransform, _bonusScale);
            canvasGroup.alpha = 1;

            if (playAnimation)
            {
                var anim = _animationContainer.GetAnimation(false);
                if (anim == null)
                {
                    anim = UnityScreenNavigatorSettings.Instance.ModalBackdropExitAnimation;
                }

                anim.Setup(rectTransform);
                yield return CoroutineManager.Run<ModalBackdrop>(anim.CreatePlayRoutine());
            }

            canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
    }
}