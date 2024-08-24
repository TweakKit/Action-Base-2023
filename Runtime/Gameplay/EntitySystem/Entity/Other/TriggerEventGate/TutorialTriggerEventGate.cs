namespace Runtime.Gameplay.EntitySystem
{
    public class TutorialTriggerEventGate : TriggerEventGate
    {
        #region Class Methods

        protected override void TriggerEnterWithMainHero(IInteractable interractable)
        {
            base.TriggerEnterWithMainHero(interractable);
            gameObject.SetActive(false);
        }

        #endregion Class Methods
    }
}