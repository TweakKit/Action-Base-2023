using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Core.Shared.Views;
using UnityScreenNavigator.Runtime.Foundation;
using UnityScreenNavigator.Runtime.Foundation.AssetLoaders;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Activities
{
    public class ActivityContainer : ContainerLayer
    {
        private readonly Dictionary<string, AssetLoadHandle<GameObject>> _preloadedResourceHandles = new();
        private readonly Dictionary<int, AssetLoadHandle<GameObject>> _assetLoadHandles = new();
        private readonly List<IActivityContainerCallbackReceiver> _callbackReceivers = new();
        private readonly Dictionary<string, Activity> _activities = new();

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

        public IReadOnlyDictionary<string, Activity> Activities => _activities;

        /// <summary>
        /// Load the data of <see cref="ActivityContainer"/>.
        /// </summary>
        public override void SetUp(IContainerLayerManager manager, string layerName)
        {
            var container = gameObject.GetOrAddComponent<ActivityContainer>();
            container.Initialize(manager, layerName);
        }

        /// <summary>
        /// Add a callback receiver.
        /// </summary>
        /// <param name="callbackReceiver"></param>
        public void AddCallbackReceiver(IActivityContainerCallbackReceiver callbackReceiver)
        {
            _callbackReceivers.Add(callbackReceiver);
        }

        /// <summary>
        /// Remove a callback receiver.
        /// </summary>
        /// <param name="callbackReceiver"></param>
        public void RemoveCallbackReceiver(IActivityContainerCallbackReceiver callbackReceiver)
        {
            _callbackReceivers.Remove(callbackReceiver);
        }

        public void Add(string resourcePath, Activity activity)
        {
            if (resourcePath == null)
                throw new ArgumentNullException(nameof(resourcePath));

            if (_activities.TryGetValue(resourcePath, out var otherActivity))
            {
                if (activity != otherActivity)
                {
                    Debug.LogError($"Another {nameof(Activity)} is existing for {resourcePath}", otherActivity);
                }

                return;
            }

            _activities.Add(resourcePath, activity);

            if (activity.TryGetTransform(out var transform))
                transform.AddChild(transform);
        }

        public bool Remove(string resourcePath)
        {
            if (resourcePath == null)
                throw new ArgumentNullException(nameof(resourcePath));

            if (_activities.TryGetValue(resourcePath, out var activity))
            {
                if (activity.TryGetTransform(out var transform))
                    transform.RemoveChild(transform);

                return _activities.Remove(resourcePath);
            }

            return false;
        }

        public bool TryGet(string resourcePath, out Activity activity)
        {
            return _activities.TryGetValue(resourcePath, out activity);
        }

        public bool TryGet<T>(string resourcePath, out T activity) where T : Activity
        {
            if (_activities.TryGetValue(resourcePath, out var otherActivity))
            {
                if (otherActivity is T activityT)
                {
                    activity = activityT;
                    return true;
                }
            }

            activity = default;
            return false;
        }

        public void Clear()
        {
            foreach (var kv in _activities)
            {
                Destroy(kv.Value.gameObject);
            }

            _activities.Clear();
        }

        protected virtual int GetChildIndex(Transform child)
        {
            Transform myTransform = transform;
            var count = myTransform.childCount;
            for (var i = count - 1; i >= 0; i--)
            {
                if (myTransform.GetChild(i).Equals(child))
                    return i;
            }

            return -1;
        }

        public AsyncProcessHandle Show<TActivity>(ActivityOptions options, params object[] args)
            where TActivity : Activity
        {
            return CoroutineManager.Run<Activity>(ShowRoutine<TActivity>(options, args));
        }

        public AsyncProcessHandle Show(ActivityOptions options, params object[] args)
        {
            return CoroutineManager.Run<Activity>(ShowRoutine<Activity>(options, args));
        }

        private IEnumerator ShowRoutine<TActivity>(ActivityOptions options, Memory<object> args)
            where TActivity : Activity
        {
            var resourcePath = options.options.resourcePath;

            if (resourcePath == null)
            {
                throw new ArgumentNullException(nameof(resourcePath));
            }

            if (_activities.TryGetValue(resourcePath, out var showingActivity))
            {
                yield break;
            }

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

            if (instance.TryGetComponent<TActivity>(out var activity) == false)
            {
                throw new InvalidOperationException(
                    $"Cannot transition because the {typeof(TActivity).Name} component is not attached to the specified resource \"{resourcePath}\".");
            }

            var activityId = activity.GetInstanceID();
            activity.Identifier = string.Concat(gameObject.name, activityId.ToString());
            _assetLoadHandles.Add(activityId, assetLoadHandle);

            Add(resourcePath, activity);

            options.options.onLoaded?.Invoke(activity);

            var afterLoadHandle = activity.AfterLoad((RectTransform)transform, args);

            while (!afterLoadHandle.IsTerminated)
            {
                yield return null;
            }

            activity.SetSortingLayer(options.sortingLayer, options.orderInLayer);

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.BeforeShow(activity);
            }

            {
                var coroutineHandle = activity.BeforeEnter(true);

                while (!coroutineHandle.IsTerminated)
                {
                    yield return coroutineHandle;
                }
            }

            // Play Animation
            {
                var coroutineHandle = activity.Enter(true, options.options.playAnimation);

                while (!coroutineHandle.IsTerminated)
                {
                    yield return coroutineHandle;
                }
            }

            // Postprocess
            activity.AfterEnter(true);

            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.AfterShow(activity);
            }

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                Interactable = true;
            }
        }

        public AsyncProcessHandle Hide(string resourcePath, bool playAnimation = true)
        {
            return CoroutineManager.Run<Activity>(HideRoutine(resourcePath, playAnimation));
        }

        public void HideAll(bool playAnimation = true)
        {
            foreach (var kv in _activities)
            {
                kv.Value.Exit(false, playAnimation);
            }
        }

        private IEnumerator HideRoutine(string resourcePath, bool playAnimation)
        {
            if (TryGet(resourcePath, out var activity) == false)
            {
                throw new InvalidOperationException(
                    $"Cannot transition because there is no activity loaded on the stack by the resource path `{resourcePath}`"
                );
            }

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                Interactable = false;
            }

            // Preprocess
            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.BeforeHide(activity);
            }

            {
                var coroutineHandle = activity.BeforeEnter(false);

                while (!coroutineHandle.IsTerminated)
                {
                    yield return coroutineHandle;
                }
            }

            // Play Animation
            {
                var coroutineHandle = activity.Enter(false, playAnimation);

                while (!coroutineHandle.IsTerminated)
                {
                    yield return coroutineHandle;
                }
            }

            // End Transition
            Remove(resourcePath);

            // Postprocess
            activity.AfterEnter(false);

            foreach (var callbackReceiver in _callbackReceivers)
            {
                callbackReceiver.AfterHide(activity);
            }

            // Unload Unused Activity
            var activityId = activity.GetInstanceID();
            var loadHandle = _assetLoadHandles[activityId];
            Destroy(activity.gameObject);
            AssetLoader.Release(loadHandle);
            _assetLoadHandles.Remove(activityId);

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                Interactable = true;
            }
        }

        public AsyncProcessHandle Preload(string resourcePath, bool loadAsync = true)
        {
            return CoroutineManager.Run<Activity>(PreloadRoutine(resourcePath, loadAsync));
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
            AssetLoader.Release(handle);
        }
    }
}