using Runtime.Definition;
using UnityEngine;
using UnityEngine.UI;

namespace UnityScreenNavigator.Runtime.Core.Modals
{
    public class TutorialContainer : ModalContainer
    {
        #region Members

        [SerializeField]
        private Button _skipCutsceneButton;

        #endregion Members

        #region Properties

        public TutorialType PlayingTutorialType { get; private set; }

        #endregion Properties

        #region API Methods

        protected override void Start()
        {
            base.Start();
            _skipCutsceneButton?.gameObject.SetActive(false);
            _skipCutsceneButton?.transform.SetAsLastSibling();
            PlayingTutorialType = TutorialType.None;
        }

        #endregion API Methods

        #region Class Methods

        public void Init(TutorialType tutorialType, bool canSkipTutorialSequence)
        {
            PlayingTutorialType = tutorialType;
            canSkipTutorialSequence = false;
            _skipCutsceneButton?.gameObject.SetActive(canSkipTutorialSequence);
            if (canSkipTutorialSequence)
            {
                _skipCutsceneButton?.onClick.RemoveAllListeners();
                _skipCutsceneButton?.onClick.AddListener(OnClickSkipCutscene);
            }
        }

        public void ResetState()
        {
            _skipCutsceneButton?.gameObject.SetActive(false);
            PlayingTutorialType = TutorialType.None;
        }

        private void OnClickSkipCutscene()
        {
            _skipCutsceneButton?.gameObject.SetActive(false);
            //TutorialNavigator.CurrentTutorial.SkipSequence();
        }

        #endregion Class Methods
    }
}