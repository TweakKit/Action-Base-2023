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

namespace UnityScreenNavigator.Runtime.Core.Screens
{
    public class ScreenContainer : ContainerLayer
    {
        private readonly Dictionary<int, AssetLoadHandle<GameObject>> _assetLoadHandles = new();
        private readonly List<IScreenContainerCallbackReceiver> _callbackReceivers = new();
        private readonly Dictionary<string, AssetLoadHandle<GameObject>> _preloadedResourceHandles = new();

        private List<Screen> _screens = new();
        private bool _isActiveScreenStacked;
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
        /// True if in transition.
        /// </summary>
        public bool IsInTransition { get; private set; }

        /// <summary>
        /// Stacked screens.
        /// </summary>
        public IReadOnlyList<Screen> Screens => _screens;

        public Window Current => _screens[^1];

        protected override void Awake()
            => _callbackReceivers.AddRange(GetComponents<IScreenContainerCallbackReceiver>());

        protected override void OnDestroy()
        {
            foreach (var screen in _screens)
            {
                var screenId = screen.GetInstanceID();
                if (_assetLoadHandles.ContainsKey(screenId))
                {
                    var assetLoadHandle = _assetLoadHandles[screenId];
                    AssetLoader.Release(assetLoadHandle);
                }
                Destroy(screen.gameObject);
            }

            _screens = null;
            _assetLoadHandles.Clear();
        }

        /// <summary>
        /// Set up the <see cref="ScreenContainer"/>.
        /// </summary>
        public override void SetUp(IContainerLayerManager manager, string layerName)
        {
            var container = gameObject.GetOrAddComponent<ScreenContainer>();
            container.Initialize(manager, layerName);
        }

        public virtual int Preload(object arg)
        {
            var preloadedScreens = gameObject.GetComponentsInChildren<Screen>();
            foreach (var preloadedScreen in preloadedScreens)
                CoroutineManager.Run<Screen>(NoPushJustLoadRoutine(preloadedScreen, arg));
            return preloadedScreens.Length;
        }

        public override void CleanUp()
        {
            foreach (var screen in _screens)
                screen.CleanUp();
        }

        public override void DestroyBeforeSceneChanged()
        {
            foreach (var screen in _screens)
                screen.DestroyBeforeSceneChanged();
        }

        /// <summary>
        /// Add a callback receiver.
        /// </summary>
        /// <param name="callbackReceiver"></param>
        public void AddCallbackReceiver(IScreenContainerCallbackReceiver callbackReceiver)
        {
            _callbackReceivers.Add(callbackReceiver);
        }

        /// <summary>
        ///     Remove a callback receiver.
        /// </summary>
        /// <param name="callbackReceiver"></param>
        public void RemoveCallbackReceiver(IScreenContainerCallbackReceiver callbackReceiver)
        {
            _callbackReceivers.Remove(callbackReceiver);
        }

        /// <summary>
        /// Push new screen.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public AsyncProcessHandle Push(ScreenOptions options, object arg)
        {
            return CoroutineManager.Run<Screen>(PushRoutine<Screen>(options, arg));
        }

        /// <summary>
        /// No push just load a screen.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public AsyncProcessHandle NoPushJustLoad(Screen screen, object arg)
        {
            return CoroutineManager.Run<Screen>(NoPushJustLoadRoutine(screen, arg));
        }

        /// <summary>
        /// Pop current screen.
        /// </summary>
        /// <param name="playAnimation"></param>
        /// <returns></returns>
        public AsyncProcessHandle Pop(bool playAnimation)
        {
            return CoroutineManager.Run<Screen>(PopRoutine(playAnimation));
        }

        private IEnumerator PushRoutine<TScreen>(ScreenOptions options, object arg)
            where TScreen : Screen
        {
            var resourcePath = options.options.resourcePath;
            if (resourcePath == null)
            {
#if DEBUGGING
                Debug.LogWarning("Attention: Nothing will be pushed! -> No resource path found <-");
#endif
                yield break;
            }

            if (IsInTransition)
            {
#if DEBUGGING
                Debug.LogWarning("Attention: It should never reach here! -> A screen push routine is in transition <-");
#endif
                yield break;
            }

            IsInTransition = true;

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                Interactable = false;
            }

            // Setup
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

            if (instance.TryGetComponent<TScreen>(out var enterScreen) == false)
            {
                throw new InvalidOperationException(
                    $"Cannot transition because {typeof(TScreen).Name} component is not attached to the specified resource \"{resourcePath}\".");
            }

            var screenId = enterScreen.GetInstanceID();
            _assetLoadHandles.Add(screenId, assetLoadHandle);
            options.options.onLoaded?.Invoke(enterScreen);

            var afterLoadHandle = enterScreen.AfterLoad((RectTransform)transform, arg);

            while (!afterLoadHandle.IsTerminated)
            {
                yield return null;
            }

            var exitScreen = _screens.Count == 0 ? null : _screens[^1];
            var exitScreenId = exitScreen == null ? (int?) null : exitScreen.GetInstanceID();

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.BeforePush(enterScreen, exitScreen);
            }

            using var preprocessHandles = new ValueList<AsyncProcessHandle>(2);
            if (exitScreen != null)
            {
                preprocessHandles.Add(exitScreen.BeforeExit(true, enterScreen));
            }

            preprocessHandles.Add(enterScreen.BeforeEnter(true, exitScreen));

            foreach (var coroutineHandle in preprocessHandles)
            {
                while (!coroutineHandle.IsTerminated)
                {
                    yield return coroutineHandle;
                }
            }

            // Play Animations
            using var animationHandles = new ValueList<AsyncProcessHandle>(2);
            if (exitScreen != null)
            {
                animationHandles.Add(exitScreen.Exit(true, options.options.playAnimation, enterScreen));
            }

            animationHandles.Add(enterScreen.Enter(true, options.options.playAnimation, exitScreen));

            foreach (var coroutineHandle in animationHandles)
            {
                while (!coroutineHandle.IsTerminated)
                {
                    yield return coroutineHandle;
                }
            }

            // End Transition
            if (!_isActiveScreenStacked && exitScreenId.HasValue)
            {
                _screens.RemoveAt(_screens.Count - 1);
            }

            _screens.Add(enterScreen);

            // Postprocess
            if (exitScreen != null)
            {
                exitScreen.AfterExit(true, enterScreen);
            }

            enterScreen.AfterEnter(true, exitScreen);

            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.AfterPush(enterScreen, exitScreen);
            }

            // Unload Unused Screen
            if (!_isActiveScreenStacked && exitScreenId.HasValue)
            {
                var beforeReleaseHandle = exitScreen.BeforeRelease();
                while (!beforeReleaseHandle.IsTerminated)
                {
                    yield return null;
                }

                var handle = _assetLoadHandles[exitScreenId.Value];
                AssetLoader.Release(handle);

                Destroy(exitScreen.gameObject);
                _assetLoadHandles.Remove(exitScreenId.Value);
            }

            _isActiveScreenStacked = options.stack;

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                Interactable = true;
            }

            IsInTransition = false;
        }

        private IEnumerator NoPushJustLoadRoutine(Screen screen, object arg)
        {
            if (IsInTransition)
            {
#if DEBUGGING
                Debug.LogWarning("Attention: It should never reach here! -> A screen push routine is in transition <-");
#endif
                yield break;
            }

            IsInTransition = true;

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
                Interactable = false;

            var enterScreen = screen;
            var afterLoadHandle = enterScreen.AfterLoad((RectTransform)transform, arg);
            while (!afterLoadHandle.IsTerminated)
                yield return null;

            var exitScreen = _screens.Count == 0 ? null : _screens[^1];
            var exitScreenId = exitScreen == null ? (int?) null : exitScreen.GetInstanceID();

            // Preprocess.
            foreach (var callbackReceiver in _callbackReceivers)
                callbackReceiver.BeforePush(enterScreen, exitScreen);

            using var preprocessHandles = new ValueList<AsyncProcessHandle>(2);
            if (exitScreen != null)
                preprocessHandles.Add(exitScreen.BeforeExit(true, enterScreen));
            preprocessHandles.Add(enterScreen.BeforeEnter(true, exitScreen));

            foreach (var coroutineHandle in preprocessHandles)
            {
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;
            }

            // Play Animations.
            using var animationHandles = new ValueList<AsyncProcessHandle>(2);
            if (exitScreen != null)
                animationHandles.Add(exitScreen.Exit(true, false, enterScreen));
            animationHandles.Add(enterScreen.Enter(true, false, exitScreen));

            foreach (var coroutineHandle in animationHandles)
            {
                while (!coroutineHandle.IsTerminated)
                    yield return coroutineHandle;
            }

            // End Transition.
            if (!_isActiveScreenStacked && exitScreenId.HasValue)
                _screens.RemoveAt(_screens.Count - 1);

            _screens.Add(enterScreen);

            // Postprocess.
            if (exitScreen != null)
                exitScreen.AfterExit(true, enterScreen);
            enterScreen.AfterEnter(true, exitScreen);

            foreach (var callbackReceiver in _callbackReceivers)
                callbackReceiver.AfterPush(enterScreen, exitScreen);

            // Unload Unused Screen.
            if (!_isActiveScreenStacked && exitScreenId.HasValue)
            {
                var beforeReleaseHandle = exitScreen.BeforeRelease();
                while (!beforeReleaseHandle.IsTerminated)
                    yield return null;

                var handle = _assetLoadHandles[exitScreenId.Value];
                AssetLoader.Release(handle);

                Destroy(exitScreen.gameObject);
                _assetLoadHandles.Remove(exitScreenId.Value);
            }

            _isActiveScreenStacked = true;

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
                Interactable = true;

            IsInTransition = false;
        }

        private IEnumerator PopRoutine(bool playAnimation)
        {
            if (_screens.Count == 0)
            {
#if DEBUGGING
                Debug.LogWarning("Attention: It should never reach here! -> No screens to pop <-");
#endif
                yield break;
            }

            if (IsInTransition)
            {
#if DEBUGGING
                Debug.LogWarning("Attention: It should never reach here! -> A screen pop routine is in transition <-");
#endif
                yield break;
            }

            IsInTransition = true;

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                Interactable = false;
            }

            var exitScreen = _screens[^1];
            var exitScreenId = exitScreen.GetInstanceID();
            var enterScreen = _screens.Count == 1 ? null : _screens[^2];

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.BeforePop(enterScreen, exitScreen);
            }

            using var preprocessHandles = new ValueList<AsyncProcessHandle>(2);
            preprocessHandles.Add(exitScreen.BeforeExit(false, enterScreen));

            if (enterScreen != null)
            {
                preprocessHandles.Add(enterScreen.BeforeEnter(false, exitScreen));
            }

            foreach (var coroutineHandle in preprocessHandles)
            {
                while (!coroutineHandle.IsTerminated)
                {
                    yield return coroutineHandle;
                }
            }

            // Play Animations
            using var animationHandles = new ValueList<AsyncProcessHandle>(2);
            animationHandles.Add(exitScreen.Exit(false, playAnimation, enterScreen));

            if (enterScreen != null)
            {
                animationHandles.Add(enterScreen.Enter(false, playAnimation, exitScreen));
            }

            foreach (var coroutineHandle in animationHandles)
            {
                while (!coroutineHandle.IsTerminated)
                {
                    yield return coroutineHandle;
                }
            }

            // End Transition
            _screens.RemoveAt(_screens.Count - 1);

            // Postprocess
            exitScreen.AfterExit(false, enterScreen);
            if (enterScreen != null)
            {
                enterScreen.AfterEnter(false, exitScreen);
            }

            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.AfterPop(enterScreen, exitScreen);
            }

            // Unload Unused Screen
            var beforeReleaseHandle = exitScreen.BeforeRelease();
            while (!beforeReleaseHandle.IsTerminated)
            {
                yield return null;
            }

            var loadHandle = _assetLoadHandles[exitScreenId];
            Destroy(exitScreen.gameObject);
            AssetLoader.Release(loadHandle);
            _assetLoadHandles.Remove(exitScreenId);

            _isActiveScreenStacked = true;

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                Interactable = true;
            }

            IsInTransition = false;
        }

        public AsyncProcessHandle Preload(string resourcePath, bool loadAsync = true)
        {
            return CoroutineManager.Run<Screen>(PreloadRoutine(resourcePath, loadAsync));
        }

        private IEnumerator PreloadRoutine(string resourcePath, bool loadAsync = true)
        {
            if (_preloadedResourceHandles.ContainsKey(resourcePath))
            {
                throw new InvalidOperationException(
                    $"The resource {resourcePath} has already been preloaded.");
            }

            var assetLoadHandle = loadAsync
                ? AssetLoader.LoadAsync<GameObject>(resourcePath)
                : AssetLoader.Load<GameObject>(resourcePath);
            _preloadedResourceHandles.Add(resourcePath, assetLoadHandle);

            if (!assetLoadHandle.IsDone)
            {
                yield return new WaitUntil(() => assetLoadHandle.IsDone);
            }

            if (assetLoadHandle.Status == AssetLoadStatus.Failed)
            {
                throw assetLoadHandle.OperationException;
            }
        }

        public bool IsPreloadRequested(string resourcePath)
        {
            return _preloadedResourceHandles.ContainsKey(resourcePath);
        }

        public bool IsPreloaded(string resourcePath)
        {
            if (!_preloadedResourceHandles.TryGetValue(resourcePath, out var handle))
            {
                return false;
            }

            return handle.Status == AssetLoadStatus.Success;
        }

        public void ReleasePreloaded(string resourcePath)
        {
            if (!_preloadedResourceHandles.ContainsKey(resourcePath))
            {
                throw new InvalidOperationException($"The resource {resourcePath} is not preloaded.");
            }

            var handle = _preloadedResourceHandles[resourcePath];
            _preloadedResourceHandles.Remove(resourcePath);
            AssetLoader.Release(handle);
        }
    }
}