using UnityEngine;

namespace UnityScreenNavigator.Runtime.Foundation.AssetLoaders
{
    [CreateAssetMenu(fileName = "ResourcesAssetLoader", menuName = "Screen Navigator/Loaders/Resources Asset Loader")]
    public sealed class ResourcesAssetLoaderObject : AssetLoaderObject, IAssetLoader
    {
        private readonly ResourcesAssetLoader _loader = new();

        public override AssetLoadHandle<T> Load<T>(string key)
        {
            return _loader.Load<T>(key);
        }

        public override AssetLoadHandle<T> LoadAsync<T>(string key)
        {
            return _loader.LoadAsync<T>(key);
        }

        public override void Release(AssetLoadHandle handle)
        {
            _loader.Release(handle);
        }
    }
}