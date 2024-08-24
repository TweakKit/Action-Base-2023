using System;
using UnityEngine;

namespace UnityScreenNavigator.Runtime.Core.Shared.Views
{
    public abstract class ContainerLayer : Window, IContainerLayer
    {
        public string LayerName { get; private set; }

        public IContainerLayerManager ContainerLayerManager { get; private set; }

        public Canvas Canvas { get; private set; }

        public abstract void SetUp(IContainerLayerManager manager, string layerName);
        public virtual void CleanUp() { }
        public virtual void DestroyBeforeSceneChanged() { }

        protected void Initialize(IContainerLayerManager manager, string layerName)
        {
            ContainerLayerManager = manager ?? throw new ArgumentNullException(nameof(manager));
            ContainerLayerManager.Add(this);
            LayerName = layerName;
            Canvas = gameObject.GetComponent<Canvas>();
        }
    }
}