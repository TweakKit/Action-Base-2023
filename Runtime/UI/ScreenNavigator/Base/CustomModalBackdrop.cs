using UnityScreenNavigator.Runtime.Core.Modals;
using Runtime.UI;
using Cysharp.Threading.Tasks;
using Runtime.Audio;

namespace Runtime.Gameplay.EntitySystem
{
    public class CustomModalBackdrop : ModalBackdrop
    {
        #region Class Methods

        protected override void PopModal()
        {
            AudioController.Instance.PlaySoundEffectAsync(AudioConstants.UI_CANCEL, this.GetCancellationTokenOnDestroy()).Forget();
            ScreenNavigator.Instance.PopModal(ownerModal, true).Forget();
        }

        #endregion Class Methods
    }
}