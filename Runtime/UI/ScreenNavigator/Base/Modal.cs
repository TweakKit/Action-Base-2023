using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Runtime.Audio;
using Runtime.Definition;
using Runtime.Manager.Data;
using Runtime.Manager.Game;
using Runtime.Manager.Toast;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Sheets;
using ScreenNavigatorModal = UnityScreenNavigator.Runtime.Core.Modals.Modal;

namespace Runtime.UI
{
    public abstract class SheetContainerModal<T> : Modal<T> where T : ModalData
    {
        #region Members

        [SerializeField] protected SheetContainer container;
        protected int[] sheetIds;
        protected bool isLoading;

        #endregion Members

        #region Properties

        protected abstract Dictionary<int, string> SheetDictionary { get; }

        #endregion Properties

        #region Class Methods

        public override UniTask Initialize(T modalData)
        {
            isLoading = false;
            sheetIds = new int[SheetDictionary.Count];
            ResetSheetIds();
            return base.Initialize(modalData);
        }

        public override UniTask CleanUp()
        {
            if (ownerModalData != null && ownerModalData.OnClosedCallbackAction != null)
                ownerModalData.OnClosedCallbackAction.Invoke();
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

        #endregion Class Methods
    }

    public abstract class Modal<T> : ScreenNavigatorModal where T : ModalData
    {
        #region Members

        [SerializeField] protected ModalType modalType = ModalType.Normal;
        [SerializeField] protected bool isDisplayedFullScreen = true;
        protected T ownerModalData;

        #endregion Members

        #region Class Methods

        public override async UniTask InitializeInternal(object arg = null)
        {
            var modalData = arg as T;
            await Initialize(modalData);
        }

        public virtual async UniTask Initialize(T modalData)
        {
            ScreenNavigator.Instance.SetUpModalOnInitialized(isDisplayedFullScreen);
            ownerModalData = modalData;
            switch (modalType)
            {
                case ModalType.Positive:
                    AudioController.Instance.PlaySoundEffectAsync(AudioConstants.POPUP_POSITIVE, this.GetCancellationTokenOnDestroy()).Forget();
                    break;
                case ModalType.Negative:
                    AudioController.Instance.PlaySoundEffectAsync(AudioConstants.POPUP_NEGATIVE, this.GetCancellationTokenOnDestroy()).Forget();
                    break;
                case ModalType.Revive:
                    AudioController.Instance.PlaySoundEffectAsync(AudioConstants.REVIVE_PROMPT, this.GetCancellationTokenOnDestroy()).Forget();
                    break;
                default:
                    AudioController.Instance.PlaySoundEffectAsync(AudioConstants.POPUP_POSITIVE, this.GetCancellationTokenOnDestroy()).Forget();
                    break;
            }

            await UniTask.CompletedTask;
        }

        public override UniTask CleanUp()
        {
            GameManager.Instance.ContinueGameFlow();
            ScreenNavigator.Instance.SetUpModalOnCleanUp();
            if (ownerModalData != null && ownerModalData.OnClosedCallbackAction != null)
                ownerModalData.OnClosedCallbackAction.Invoke();
            return base.CleanUp();
        }

        public virtual void Close(bool playAnimation)
        {
            if (IsActive())
                ScreenNavigator.Instance.PopModal(this, playAnimation).Forget();
        }

        public virtual void ShowToast(string toastMessage, ToastVisualType toastVisualType = ToastVisualType.Text)
            => ToastManager.Instance.Show(toastMessage, toastVisualType);

        #endregion Class Methods
    }

    public class ModalData
    {
        #region Properties

        public Action OnClosedCallbackAction { get; private set; }

        #endregion Properties

        #region Class Methods

        public ModalData(Action onClosedCallbackAction)
            => OnClosedCallbackAction = onClosedCallbackAction;

        #endregion Class Methods
    }
}