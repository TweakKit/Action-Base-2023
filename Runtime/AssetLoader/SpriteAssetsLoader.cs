using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Runtime.Common.Singleton;
using Cysharp.Threading.Tasks;

namespace Runtime.AssetLoader
{
    public class SpriteAssetsLoader : MonoSingleton<SpriteAssetsLoader>
    {
        #region Members

        private Dictionary<string, Sprite> _spriteAssetsDictionary;

        #endregion Members

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            _spriteAssetsDictionary = new Dictionary<string, Sprite>();
        }

        #endregion API Methods

        #region Class Methods

        public static async UniTask<Sprite> LoadAsset(string assetId, CancellationToken cancellationToken)
        {
            Sprite assetSprite = null;
            if (!Instance._spriteAssetsDictionary.ContainsKey(assetId))
            {
                assetSprite = await Addressables.LoadAssetAsync<Sprite>(assetId).WithCancellation(cancellationToken);
                if (!Instance._spriteAssetsDictionary.ContainsKey(assetId))
                    Instance._spriteAssetsDictionary.TryAdd(assetId, assetSprite);
            }
            else assetSprite = Instance._spriteAssetsDictionary[assetId];
            return assetSprite;
        }

        #endregion Class Methods
    }
}