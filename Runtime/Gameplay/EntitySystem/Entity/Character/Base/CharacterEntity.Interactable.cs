using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Definition;
using Runtime.Extensions;
using Runtime.Manager.Pool;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract partial class CharacterEntity<T> : Entity<T>, IInteractable where T : CharacterModel
    {
        #region Members

        private static string s_vfxHolderName = "vfx_holder";
        private static string s_vfxTopPositionName = "top_position";
        private static string s_vfxBottomPositionName = "bottom_position";
        private static string s_vfxMiddlePositionName = "middle_position";
        private Transform _topPosition;
        private Transform _bottomPosition;
        private Transform _middlePosition;
        private List<IStatusEffect> _statusEffects = new List<IStatusEffect>();
        private Dictionary<StatusEffectType, StatusEffectVFX> _statusEffectVFXsDictionary;

        #endregion Members

        #region Properties

        public EntityModel Model
            => ownerModel;

        public virtual bool IsMainHero => false;

        #endregion Properties

        #region Class Methods

        public abstract bool CanGetAffected(EntityModel interactingModel, DamageSource damageSource);

        public virtual void GetHit(DamageInfo damageInfo, Vector2 damageDirection)
        {
            var evasionRate = ownerModel.GetTotalStatValue(StatType.Evasion);
            if (evasionRate >= Random.Range(0.0f, 1.0f))
            {
                ownerModel.MissedDamage();
            }
            else
            {
                ownerModel.DebuffHp(damageInfo);
                if (!ownerModel.IsDead && damageInfo.damageStatusEffectModels != null)
                    GetAffected(damageInfo.creatorModel, damageInfo.damageStatusEffectModels, damageDirection);
            }
        }

        public virtual void GetAffected(EntityModel senderModel, StatusEffectModel[] statusEffectModels, Vector2 affectDirection)
        {
            foreach (var statusEffectModel in statusEffectModels)
                CauseAffect(statusEffectModel, senderModel, affectDirection);
        }

        protected void CauseAffect(StatusEffectModel statusEffectModel, EntityModel senderModel, Vector2 affectDirection)
        {
            if (CanIgnoreStatusEffect(statusEffectModel.StatusEffectType))
                return;

            if (CanStopCurrentStatusEffects(statusEffectModel.StatusEffectType))
                StopStatusEffects(statusEffectModel.StatusEffectType);

            if (statusEffectModel.StatusEffectType == StatusEffectType.NegativeStatusEffectRemove)
                RemoveNegativeStatusEffects();

            var newStatusEffectModel = statusEffectModel.Clone();
            if (newStatusEffectModel.IsAffectable)
            {
                var numberOfSameStatusEffects = 0;
                var newStatusEffectType = newStatusEffectModel.StatusEffectType;
                numberOfSameStatusEffects = _statusEffects.Count(x => x.StatusEffectType == newStatusEffectType);
                if (newStatusEffectModel.IsStackable)
                {
                    // Stack status effect: The new status effect will always replace the old one.
                    if (numberOfSameStatusEffects > 0)
                    {
                        var oldStatusEffect = _statusEffects.First(x => x.StatusEffectType == newStatusEffectType);
                        newStatusEffectModel.Stack(oldStatusEffect.StatusEffectModel);
                        oldStatusEffect.Stop();
                        _statusEffects.Remove(oldStatusEffect);
                    }

                    ownerModel.StackStatusEffect(newStatusEffectType);
                }
                else
                {
                    // Stop all old same type status effects.
                    if (numberOfSameStatusEffects > 0)
                    {
                        for (int i = _statusEffects.Count - 1; i >= 0; i--)
                        {
                            if (_statusEffects[i].StatusEffectType == newStatusEffectType)
                            {
                                _statusEffects[i].Stop();
                                _statusEffects.Remove(_statusEffects[i]);
                            }
                        }
                    }

                    ownerModel.StackStatusEffect(newStatusEffectType);
                }

                // Create a new status effect.
                var newStatusEffect = StatusEffectFactory.GetStatusEffect(newStatusEffectModel.StatusEffectType);
                newStatusEffect.Init(newStatusEffectModel, senderModel, ownerModel, affectDirection);
                _statusEffects.Add(newStatusEffect);

                if (numberOfSameStatusEffects == 0 && !_statusEffectVFXsDictionary.ContainsKey(newStatusEffect.StatusEffectType))
                {
                    string statusEffectVFXPrefabName = StatusEffectKey.GetStatusEffectPrefabName(newStatusEffect.StatusEffectType);
                    if (!string.IsNullOrEmpty(statusEffectVFXPrefabName))
                        CreateStatusEffectVFX(newStatusEffectModel, statusEffectVFXPrefabName).Forget();
                }
            }
        }

        protected virtual partial void PartialValidateInteractable()
        {
            var vfxHolder = transform.FindChildTransform(s_vfxHolderName);
            if (vfxHolder == null)
            {
                Debug.LogError("VFX holder's name is not mapped!");
                return;
            }
        }

        protected virtual partial void PartialDisposeInteractable()
        {
            for (int i = _statusEffects.Count - 1; i >= 0; i--)
            {
                var statusEffect = _statusEffects[i];
                statusEffect.Update();
                var numberEffectsSameType = _statusEffects.Count(x => x.StatusEffectType == statusEffect.StatusEffectType);
                if (numberEffectsSameType == 1 && _statusEffectVFXsDictionary.ContainsKey(statusEffect.StatusEffectType))
                {
                    var statusEffectVFX = _statusEffectVFXsDictionary[statusEffect.StatusEffectType];
                    statusEffectVFX.Dispose();
                    _statusEffectVFXsDictionary.Remove(statusEffect.StatusEffectType);
                }

                ownerModel.ClearStatusEffectStack(statusEffect.StatusEffectType);
                if (_statusEffects.Count > i)
                    _statusEffects.RemoveAt(i);
            }
        }

        protected virtual partial void PartialInitializeInteractable()
        {
            var vfxHolder = transform.FindChildTransform(s_vfxHolderName);
            _topPosition = vfxHolder.Find(s_vfxTopPositionName);
            _bottomPosition = vfxHolder.Find(s_vfxBottomPositionName);
            _middlePosition = vfxHolder.Find(s_vfxMiddlePositionName);
            _statusEffectVFXsDictionary = new Dictionary<StatusEffectType, StatusEffectVFX>();
        }

        protected virtual partial void PartialUpdateInteractable()
        {
            for (int i = _statusEffects.Count - 1; i >= 0; i--)
            {
                if (ownerModel.IsDead)
                    break;
                var statusEffect = _statusEffects[i];
                statusEffect.Update();
                if (statusEffect.HasFinished)
                {
                    var numberEffectsSameType = _statusEffects.Count(x => x.StatusEffectType == statusEffect.StatusEffectType);
                    if (numberEffectsSameType == 1 && _statusEffectVFXsDictionary.ContainsKey(statusEffect.StatusEffectType))
                    {
                        var statusEffectVFX = _statusEffectVFXsDictionary[statusEffect.StatusEffectType];
                        statusEffectVFX.Dispose();
                        _statusEffectVFXsDictionary.Remove(statusEffect.StatusEffectType);
                    }

                    ownerModel.ClearStatusEffectStack(statusEffect.StatusEffectType);
                    if (_statusEffects.Count > i)
                        _statusEffects.RemoveAt(i);
                }
            }
        }

        private async UniTaskVoid CreateStatusEffectVFX(StatusEffectModel statusEffectModel, string statusEffectPrefabName)
        {
            var statusEffectVFXGameObject = await PoolManager.Instance.Get(statusEffectPrefabName, cancellationToken: transform.GetCancellationTokenOnDestroy());
            if (ownerModel.IsDead)
            {
                PoolManager.Instance.Remove(statusEffectVFXGameObject);
                return;
            }
            
            var statusEffectVFX = statusEffectVFXGameObject.GetOrAddComponent<StatusEffectVFX>();
            statusEffectVFXGameObject.transform.SetParent(transform);
            statusEffectVFXGameObject.transform.localPosition = GetStatusEffectPosition(statusEffectModel.StatusEffectType);
            if (!statusEffectModel.IsOneShot)
                _statusEffectVFXsDictionary.TryAdd(statusEffectModel.StatusEffectType, statusEffectVFX);
        }

        private Vector3 GetStatusEffectPosition(StatusEffectType statusEffectType)
        {
            switch (statusEffectType)
            {
                case StatusEffectType.Stun:
                case StatusEffectType.Heal:
                case StatusEffectType.HealAttack:
                case StatusEffectType.HealingAttackDuration:
                case StatusEffectType.Taunt:
                case StatusEffectType.AttackBuff:
                case StatusEffectType.Berserker:
                case StatusEffectType.DecreaseTakeHp:
                case StatusEffectType.GetMoreDamage:
                    return _topPosition.localPosition;

                case StatusEffectType.BleedAttack:
                case StatusEffectType.PoisonAttack:
                case StatusEffectType.BurnAttack:
                case StatusEffectType.DamageReductionBuff:
                case StatusEffectType.FixedDamageReductionBuff:
                case StatusEffectType.DamageReductionNonDuration:
                case StatusEffectType.FixedDamageReductionNonDuration:
                case StatusEffectType.CritChanceBuff:
                case StatusEffectType.EvasionBuff:
                case StatusEffectType.Quick:
                case StatusEffectType.Haste:
                case StatusEffectType.Slow:
                case StatusEffectType.Invincibility:
                case StatusEffectType.Gargoyle:
                case StatusEffectType.Goku:
                    return _middlePosition.localPosition;

                case StatusEffectType.Chill:
                case StatusEffectType.Freeze:
                case StatusEffectType.NegativeStatusEffectRemove:
                case StatusEffectType.Shield:
                    return _bottomPosition.localPosition;

                default:
                    return _topPosition.localPosition;
            }
        }

        private bool CanIgnoreStatusEffect(StatusEffectType newStatusEffectType)
        {
            if (newStatusEffectType == StatusEffectType.Chill)
            {
                foreach (var statusEffect in _statusEffects)
                {
                    if (statusEffect.StatusEffectType == StatusEffectType.Freeze)
                        return true;
                }
            }

            return false;
        }

        private bool CanStopCurrentStatusEffects(StatusEffectType newStatusEffectType)
        {
            if (newStatusEffectType == StatusEffectType.Freeze)
            {
                foreach (var statusEffect in _statusEffects)
                {
                    if (statusEffect.StatusEffectType == StatusEffectType.Chill)
                        return true;
                }
            }

            return false;
        }

        private void StopStatusEffects(StatusEffectType newStatusEffectType)
        {
            if (newStatusEffectType == StatusEffectType.Freeze)
            {
                foreach (var statusEffect in _statusEffects)
                {
                    if (statusEffect.StatusEffectType == StatusEffectType.Chill)
                        statusEffect.Stop();
                }
            }
        }

        private void RemoveNegativeStatusEffects()
        {
            foreach (var statusEffect in _statusEffects)
            {
                if (IsNegativeStatusEffect(statusEffect.StatusEffectType))
                    statusEffect.Stop();
            }
        }

        private bool IsNegativeStatusEffect(StatusEffectType statusEffectType)
        {
            return statusEffectType == StatusEffectType.Stun || statusEffectType == StatusEffectType.BleedAttack ||
                   statusEffectType == StatusEffectType.PoisonAttack || statusEffectType == StatusEffectType.BurnAttack ||
                   statusEffectType == StatusEffectType.Chill || statusEffectType == StatusEffectType.Freeze ||
                   statusEffectType == StatusEffectType.KnockUp || statusEffectType == StatusEffectType.Slow;
        }

        #endregion Class Methods
    }
}