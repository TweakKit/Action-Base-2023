using Runtime.Definition;
using Runtime.Message;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class TutorialTriggerObject : MonoBehaviour
    {
        #region Members

        [SerializeField]
        private int _id;
        [SerializeField]
        private TutorialTriggerType _tutorialDialogTriggerType;

        #endregion Members

        #region API Methods

        private void OnTriggerEnter2D(Collider2D collider)
        {
            var interractable = collider.gameObject.GetComponent<IInteractable>();
            if (interractable != null && interractable.IsMainHero)
            {
                Messenger.Publish(new TutorialDialogTriggeredMessage(_id, _tutorialDialogTriggerType, transform.position));
                Hide();
            }
        }

        #endregion API Methods

        #region Class Methods

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        #endregion Class Methods
    }
}