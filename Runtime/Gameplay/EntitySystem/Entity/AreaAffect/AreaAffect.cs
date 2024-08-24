using System;
using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [RequireComponent(typeof(Collider2D))]
    public class AreaAffect : MonoBehaviour
    {
        #region Members

        [SerializeField]
        protected Collider2D colliderSelf;
        protected AreaAffectModel ownerModel;
        protected float currentAffectDuration;
        protected float duration;
        protected EntityType teammateTargetType;
        protected EntityType rivalsTargetType;
        protected EntityModel senderModel;
        protected StatusEffectModel[] statusEffectModel;
        protected Action onDestroyCallback;
        protected bool isInited = false;

        #endregion Members

        #region Properties

        protected bool HasFinished { get; private set; }

        #endregion Properties

        #region Class Methods

        public virtual void InitAreaAffect(EntityModel senderModel, AreaAffectModel areaAffectModel, StatusEffectModel[] statusEffectModel = null, Transform parent = null, Action onDestroyCallback = null)
        {
            HasFinished = false;
            this.senderModel = senderModel;
            ownerModel = areaAffectModel;
            transform.position = areaAffectModel.Position;
            transform.parent = parent;
            duration = areaAffectModel.Duration;
            currentAffectDuration = duration;
            teammateTargetType = areaAffectModel.TeammateTargetType;
            rivalsTargetType = areaAffectModel.RivalsTargetType;
            colliderSelf.enabled = true;
            this.statusEffectModel = statusEffectModel;
            this.onDestroyCallback = onDestroyCallback;
            isInited = true;
        }

        protected virtual void OnTriggerAffect(IInteractable interactable)
        {
            var affectDirection = senderModel.Position - interactable.Position;
            interactable.GetAffected(senderModel, statusEffectModel, affectDirection);
            var characterModel = interactable.Model as CharacterModel;
            if (!characterModel.IsInDamageReductionAreaAffect)
            {
                characterModel.IsInDamageReductionAreaAffect = true;
            }
        }

        protected virtual void OutTriggerAffect(IInteractable interactable)
        {
            var characterModel = interactable.Model as CharacterModel;
            if (characterModel.IsInDamageReductionAreaAffect)
            {
                characterModel.IsInDamageReductionAreaAffect = false;
            }
        }

        #endregion Class Methods

        #region API Methods

        protected virtual void Update()
        {
            if (isInited)
            {
                currentAffectDuration -= Time.unscaledDeltaTime;
                if (currentAffectDuration <= 0 && !HasFinished)
                {
                    colliderSelf.enabled = false;
                    onDestroyCallback?.Invoke();
                    HasFinished = true;
                }
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {
            if (ownerModel == null)
                return;
            var interactable = collider.GetComponent<IInteractable>();
            if (interactable != null && interactable.Model.EntityType == teammateTargetType && !interactable.Model.IsDead)
            {
                OnTriggerAffect(interactable);
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D collider)
        {
            if (ownerModel == null)
                return;
            var interactable = collider.GetComponent<IInteractable>();
            if (interactable != null && interactable.Model.EntityType == teammateTargetType && !interactable.Model.IsDead)
            {
                OutTriggerAffect(interactable);
            }
        }

        public virtual void ForceFinish()
        {
            colliderSelf.enabled = false;
            HasFinished = true;
        }

        #endregion API Methods
    }
}