using Cysharp.Threading.Tasks;
using Runtime.Audio;
using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class HeroEntity : CharacterEntity<HeroModel>
    {
        #region Properties

        public override bool IsMainHero
            => ownerModel.FollowingModel == null;

        #endregion Properties

        #region Class Methods

        public override bool CanGetAffected(EntityModel interactingModel, DamageSource damageSource)
        {
            if (!ownerModel.IsDead)
                return interactingModel.EntityType.IsEnemyOrBoss();
            else
                return false;
        }

        public override void GetHit(DamageInfo damageInfo, Vector2 damageDirection)
        {
            AudioController.Instance.PlaySoundEffectAsync(AudioConstants.GENERAL_GET_HIT, this.GetCancellationTokenOnDestroy()).Forget();
            base.GetHit(damageInfo, damageDirection);
        }

        #endregion Class Methods
    }
}