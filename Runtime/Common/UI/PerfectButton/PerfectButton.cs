using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Runtime.Common.Interface;
using Runtime.Definition;
using Runtime.Audio;
using Cysharp.Threading.Tasks;
using Runtime.Tracking;
using Runtime.Extensions;

namespace Runtime.Common.UI
{
    public class PerfectButton : Button, IClickable
    {
        #region Members

        private static readonly float s_proceedOnClickDelayTime = 0.1f;
        private static readonly float s_preventSpamDelayTime = 0.15f;
        private bool _blockInput = false;

        public ButtonSourceType buttomSourceType = ButtonSourceType.Other;
        public ButtonType buttonType = ButtonType.Other;

        #endregion Members

        #region API Methods

        protected override void OnEnable()
        {
            base.OnEnable();
            _blockInput = false;
        }

        #endregion API Methods

        #region Class Methods

        public void Click()
            => Trigger();

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!_blockInput && interactable)
            {
                _blockInput = true;
                Press();
                StartCoroutine(BlockInputTemporarily());
            }
        }

        protected virtual bool Press()
        {
            if (!IsActive())
                return false;

            StartCoroutine(InvokeOnClickAction());
            return true;
        }

        private IEnumerator InvokeOnClickAction()
        {
            yield return new WaitForSecondsRealtime(s_proceedOnClickDelayTime);
            ProceedAction();
        }

        private IEnumerator BlockInputTemporarily()
        {
            yield return new WaitForSecondsRealtime(s_preventSpamDelayTime);
            _blockInput = false;
        }

        private void ProceedAction()
        {
            onClick.Invoke();

#if TRACKING
            if (buttomSourceType != ButtonSourceType.Other)
                FirebaseManager.Instance.TrackButtonClick(buttomSourceType.ToString().ToSnakeCase(), buttonType.ToString().ToSnakeCase(), 0);
#endif

            switch (buttonType)
            {
                case ButtonType.PlayGameSplashArt:
                case ButtonType.PlayStage:
                case ButtonType.ReplayInStage:
                case ButtonType.NextStage:
                    AudioController.Instance.PlaySoundEffectAsync(AudioConstants.START_GAME, this.GetCancellationTokenOnDestroy()).Forget();
                    break;

                case ButtonType.Ok:
                case ButtonType.Yes:
                case ButtonType.Confirm:
                case ButtonType.Continue:
                    AudioController.Instance.PlaySoundEffectAsync(AudioConstants.UI_CONFIRM, this.GetCancellationTokenOnDestroy()).Forget();
                    break;

                case ButtonType.Cancel:
                case ButtonType.Back:
                case ButtonType.Close:
                case ButtonType.No:
                    AudioController.Instance.PlaySoundEffectAsync(AudioConstants.UI_CANCEL, this.GetCancellationTokenOnDestroy()).Forget();
                    break;

                case ButtonType.Upgrade:
                    //AudioController.Instance.PlaySoundEffectAsync(AudioConstants.UPGRADE_SELECT, this.GetCancellationTokenOnDestroy()).Forget();
                    break;

                case ButtonType.GachaBasicX1:
                    break;

                case ButtonType.GachaPremiumX1:
                    break;

                case ButtonType.GachaBasicX10:
                    break;

                case ButtonType.GachaPremiumX10:
                    break;

                case ButtonType.Unidentified:
                    break;

                case ButtonType.Pause:
                    AudioController.Instance.PlaySoundEffectAsync(AudioConstants.GAME_PAUSE, this.GetCancellationTokenOnDestroy()).Forget();
                    break;

                default:
                    AudioController.Instance.PlaySoundEffectAsync(AudioConstants.UI_CLICK, this.GetCancellationTokenOnDestroy()).Forget();
                    break;
            }
        }

        #endregion Class Methods

        #region Unity Event Callback Methods

        public void Trigger()
            => ProceedAction();

        #endregion Unity Event Callback Methods
    }
}