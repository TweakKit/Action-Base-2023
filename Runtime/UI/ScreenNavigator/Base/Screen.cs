using System;
using System.Collections.Generic;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Sheets;
using Runtime.Manager.Toast;
using Runtime.Manager.Game;
using Cysharp.Threading.Tasks;
using ScreenNavigatorScreen = UnityScreenNavigator.Runtime.Core.Screens.Screen;

namespace Runtime.UI
{
    public abstract class SheetContainerScreen<T> : Screen<T> where T : ScreenData
    {
        #region Members

        [SerializeField]
        protected SheetContainer container;
        protected int[] sheetIds;
        protected bool isLoading;

        #endregion Members

        #region Properties

        protected abstract Dictionary<int, string> SheetDictionary { get; }

        #endregion Properties

        public override async UniTask Initialize(T screenData)
        {
            isLoading = false;
            sheetIds = new int[SheetDictionary.Count];
            ResetSheetIds();
            await base.Initialize(screenData);
        }

        public override UniTask CleanUp()
        {
            if (ownerScreenData != null && ownerScreenData.OnClosedCallbackAction != null)
                ownerScreenData.OnClosedCallbackAction.Invoke();
            ResetSheetIds();
            return base.CleanUp();
        }

        protected void ResetSheetIds()
        {
            for (int i = 0; i < sheetIds.Length; i++)
                sheetIds[i] = -1;
        }

        public async UniTask LoadSheetByIndex(int index, bool isOn, params object[] args)
        {
            if (isOn && !isLoading)
            {
                if (container.ActiveSheetId != sheetIds[index])
                {
                    isLoading = true;
                    if (sheetIds[index] == -1)
                    {
                        var sheetName = SheetDictionary[index];
                        await container.Register(new SheetOptions(sheetName, (sheetId, sheet) => sheetIds[index] = sheetId), args);
                    }

                    await container.Show(sheetIds[index], false);
                    isLoading = false;
                }
            }
        }
    }

    public abstract class Screen<T> : ScreenNavigatorScreen where T : ScreenData
    {
        #region Members

        [SerializeField]
        protected bool isPreloaded;
        [SerializeField]
        protected bool isDisplayedFullScreen = true;
        protected T ownerScreenData;

        #endregion Members

        #region Class Methods

        public override async UniTask InitializeInternal(object arg)
        {
            var screenData = arg as T;
            await Initialize(screenData);
        }

        public virtual async UniTask Initialize(T screenData)
        {
            if (!isPreloaded)
                GameManager.Instance.StopGameFlow();
            ScreenNavigator.Instance.SetUpScreenOnInitialized(isDisplayedFullScreen);
            ownerScreenData = screenData;
            await UniTask.CompletedTask;
        }

        public override UniTask CleanUp()
        {
            if (!isPreloaded)
                GameManager.Instance.ContinueGameFlow();
            ScreenNavigator.Instance.SetUpScreenOnCleanUp();
            if (ownerScreenData != null && ownerScreenData.OnClosedCallbackAction != null)
                ownerScreenData.OnClosedCallbackAction.Invoke();
            return base.CleanUp();
        }

        public virtual void Close(bool playAnimation)
        {
            if (IsActive())
                ScreenNavigator.Instance.PopScreen(playAnimation).Forget();
        }

        public virtual void ShowToast(string toastMessage, ToastVisualType toastVisualType = ToastVisualType.Text)
            => ToastManager.Instance.Show(toastMessage, toastVisualType);

        #endregion Class Methods
    }

    public class ScreenData
    {
        #region Properties

        public Action OnClosedCallbackAction { get; private set; }

        #endregion Properties

        #region Class Methods

        public ScreenData(Action onClosedCallbackAction)
            => OnClosedCallbackAction = onClosedCallbackAction;

        #endregion Class Methods
    }
}