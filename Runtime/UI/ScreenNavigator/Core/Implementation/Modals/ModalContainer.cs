using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Core.Shared.Views;
using UnityScreenNavigator.Runtime.Foundation;
using UnityScreenNavigator.Runtime.Foundation.AssetLoaders;
using UnityScreenNavigator.Runtime.Foundation.Collections;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Modals
{
    public class ModalContainer : ContainerLayer
    {
        [SerializeField]
        private ModalBackdrop _overrideBackdropPrefab;

        private readonly Dictionary<int, AssetLoadHandle<GameObject>> _assetLoadHandles = new();
        private readonly List<IModalContainerCallbackReceiver> _callbackReceivers = new();
        private readonly Dictionary<string, AssetLoadHandle<GameObject>> _preloadedResourceHandles = new();

        private List<ModalBackdrop> _backdrops = new();
        private List<Modal> _modals = new();
        private ModalBackdrop _backdropPrefab;
        private IAssetLoader _assetLoader;

        /// <summary>
        ///     By default, <see cref="IAssetLoader" /> in <see cref="UnityScreenNavigatorSettings" /> is used.
        ///     If this property is set, it is used instead.
        /// </summary>
        public IAssetLoader AssetLoader
        {
            get => _assetLoader ?? UnityScreenNavigatorSettings.Instance.AssetLoader;
            set => _assetLoader = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        ///     True if in transition.
        /// </summary>
        public bool IsInTransition { get; private set; }

        /// <summary>
        ///     Stacked modals.
        /// </summary>
        public IReadOnlyList<Modal> Modals => _modals;

        public Window Current => _modals[^1];

        protected override void Awake()
        {
            _callbackReceivers.AddRange(GetComponents<IModalContainerCallbackReceiver>());
            _backdropPrefab = _overrideBackdropPrefab
                            ? _overrideBackdropPrefab
                            : UnityScreenNavigatorSettings.Instance.ModalBackdropPrefab;
        }

        protected override void OnDestroy()
        {
            foreach (var modal in _modals)
            {
                var modalId = modal.GetInstanceID();
                var assetLoadHandle = _assetLoadHandles[modalId];

                Destroy(modal.gameObject);
                AssetLoader.Release(assetLoadHandle);
            }

            _modals = null;
            _assetLoadHandles.Clear();
        }

        /// <summary>
        /// Set up the <see cref="ModalContainer" />.
        /// </summary>
        public override void SetUp(IContainerLayerManager manager, string layerName)
        {
            var container = gameObject.GetOrAddComponent<ModalContainer>();
            container.Initialize(manager, layerName);
        }

        public override void CleanUp()
        {
            foreach (var modal in _modals)
                modal.CleanUp();
        }

        public override void DestroyBeforeSceneChanged()
        {
            foreach (var modal in _modals)
                modal.DestroyBeforeSceneChanged();

            foreach (var backdrop in _backdrops)
                backdrop.DestroyBeforeSceneChanged();
        }

        /// <summary>
        ///     Add a callback receiver.
        /// </summary>
        /// <param name="callbackReceiver"></param>
        public void AddCallbackReceiver(IModalContainerCallbackReceiver callbackReceiver)
        {
            _callbackReceivers.Add(callbackReceiver);
        }

        /// <summary>
        ///     Remove a callback receiver.
        /// </summary>
        /// <param name="callbackReceiver"></param>
        public void RemoveCallbackReceiver(IModalContainerCallbackReceiver callbackReceiver)
        {
            _callbackReceivers.Remove(callbackReceiver);
        }

        /// Push new modal.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public AsyncProcessHandle Push(ModalOptions options, object arg)
        {
            return CoroutineManager.Run<Modal>(PushRoutine<Modal>(options, arg));
        }

        /// <summary>
        /// Pop current modal.
        /// </summary>
        /// <param name="playAnimation"></param>
        /// <returns></returns>
        public AsyncProcessHandle Pop(bool playAnimation)
        {
            return CoroutineManager.Run<Modal>(PopRoutine(playAnimation));
        }

        private IEnumerator PushRoutine<TModal>(ModalOptions options, object arg)
            where TModal : Modal
        {
            var resourcePath = options.options.resourcePath;
            if (resourcePath == null)
                throw new ArgumentNullException(nameof(resourcePath));

            if (IsInTransition)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Attention: It should never reach here! -> A modal push routine is in transition <-");
#endif
                yield break;
            }

            IsInTransition = true;

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                Interactable = false;
            }

            var assetLoadHandle = options.options.loadAsync
                ? AssetLoader.LoadAsync<GameObject>(resourcePath)
                : AssetLoader.Load<GameObject>(resourcePath);

            if (!assetLoadHandle.IsDone)
            {
                yield return new WaitUntil(() => assetLoadHandle.IsDone);
            }

            if (assetLoadHandle.Status == AssetLoadStatus.Failed)
            {
                throw assetLoadHandle.OperationException;
            }

            var instance = Instantiate(assetLoadHandle.Result);
            if (instance.TryGetComponent<TModal>(out var enterModal) == false)
            {
                throw new InvalidOperationException(
                    $"Cannot transition because {typeof(TModal).Name} component is not attached to the specified resource \"{resourcePath}\".");
            }

            var backdrop = Instantiate(_backdropPrefab);
            backdrop.Setup((RectTransform)transform, options.backdropAlpha);
            _backdrops.Add(backdrop);

            var modalId = enterModal.GetInstanceID();
            _assetLoadHandles.Add(modalId, assetLoadHandle);
            options.options.onLoaded?.Invoke(enterModal);

            var afterLoadHandle = enterModal.AfterLoad((RectTransform)transform, arg);

            while (!afterLoadHandle.IsTerminated)
            {
                yield return null;
            }

            var exitModal = _modals.Count == 0 ? null : _modals[^1];

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.BeforePush(enterModal, exitModal);
            }

            using var preprocessHandles = new ValueList<AsyncProcessHandle>(2);
            if (exitModal != null)
            {
                preprocessHandles.Add(exitModal.BeforeExit(true, enterModal));
            }

            preprocessHandles.Add(enterModal.BeforeEnter(true, exitModal));

            foreach (var coroutineHandle in preprocessHandles)
            {
                while (!coroutineHandle.IsTerminated)
                {
                    yield return coroutineHandle;
                }
            }

            // Play Animation
            using var animationHandles = new ValueList<AsyncProcessHandle>(2);
            animationHandles.Add(backdrop.Enter(options.options.playAnimation));

            if (exitModal != null)
            {
                animationHandles.Add(exitModal.Exit(true, options.options.playAnimation, enterModal));
            }

            animationHandles.Add(enterModal.Enter(true, options.options.playAnimation, exitModal));

            foreach (var coroutineHandle in animationHandles)
            {
                while (!coroutineHandle.IsTerminated)
                {
                    yield return coroutineHandle;
                }
            }

            // End Transition
            backdrop.SetOwnerModal(enterModal);
            _modals.Add(enterModal);

            // Postprocess
            if (exitModal != null)
            {
                exitModal.AfterExit(true, enterModal);
            }

            enterModal.AfterEnter(true, exitModal);

            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.AfterPush(enterModal, exitModal);
            }

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                Interactable = true;
            }

            IsInTransition = false;
        }

        private IEnumerator PopRoutine(bool playAnimation)
        {
            if (_modals.Count == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Attention: It should never reach here! -> No modals to pop <-");
#endif
                yield break;
            }

            if (IsInTransition)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Attention: It should never reach here! -> A modal pop routine is in transition <-");
#endif
                yield break;
            }

            IsInTransition = true;

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                Interactable = false;
            }

            var exitModal = _modals[^1];
            var exitModalId = exitModal.GetInstanceID();
            var enterModal = _modals.Count == 1 ? null : _modals[^2];
            var backdrop = _backdrops[^1];
            _backdrops.RemoveAt(_backdrops.Count - 1);

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.BeforePop(enterModal, exitModal);
            }

            using var preprocessHandles = new ValueList<AsyncProcessHandle>(2);
            preprocessHandles.Add(exitModal.BeforeExit(false, enterModal));

            if (enterModal != null)
            {
                preprocessHandles.Add(enterModal.BeforeEnter(false, exitModal));
            }

            foreach (var coroutineHandle in preprocessHandles)
            {
                while (!coroutineHandle.IsTerminated)
                {
                    yield return coroutineHandle;
                }
            }

            // Play Animation
            using var animationHandles = new ValueList<AsyncProcessHandle>(2);
            animationHandles.Add(exitModal.Exit(false, playAnimation, enterModal));

            if (enterModal != null)
            {
                animationHandles.Add(enterModal.Enter(false, playAnimation, exitModal));
            }

            animationHandles.Add(backdrop.Exit(playAnimation));

            foreach (var coroutineHandle in animationHandles)
            {
                while (!coroutineHandle.IsTerminated)
                {
                    yield return coroutineHandle;
                }
            }

            // End Transition
            _modals.RemoveAt(_modals.Count - 1);

            // Postprocess
            exitModal.AfterExit(false, enterModal);
            if (enterModal != null)
            {
                enterModal.AfterEnter(false, exitModal);
            }

            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.AfterPop(enterModal, exitModal);
            }

            // Unload Unused Modal
            var beforeReleaseHandle = exitModal.BeforeRelease();
            while (!beforeReleaseHandle.IsTerminated)
            {
                yield return null;
            }

            var loadHandle = _assetLoadHandles[exitModalId];
            Destroy(exitModal.gameObject);
            Destroy(backdrop.gameObject);
            AssetLoader.Release(loadHandle);
            _assetLoadHandles.Remove(exitModalId);

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                Interactable = true;
            }

            IsInTransition = false;
        }
    }
}