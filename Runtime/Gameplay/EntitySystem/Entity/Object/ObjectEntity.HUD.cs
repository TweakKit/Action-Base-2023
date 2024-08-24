using Runtime.Definition;
using Runtime.Gameplay.Visual;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class ObjectEntity : Entity<ObjectModel>
    {
        #region Class Methods

        private partial void PartialDisposeHUD() { }
        private partial void PartialValidateHUD() { }

        private partial void PartialInitializeHUD()
            => ownerModel.HealthChangedEvent += OnHandleHUDHealthChanged;

        private void OnHandleHUDHealthChanged(float deltaHp)
            => ShowDamageVisual(deltaHp);

        private void ShowDamageVisual(float deltaHp)
        {
            int damageValue = (int)deltaHp;
            GameplayVisualController.Instance.DislayDamageVisual(VFXKey.OBJECT_DAMAGE_VISUAL, 
                                                                 damageValue, 
                                                                 false,
                                                                 ownerModel.TopPosition, 
                                                                 this.GetCancellationTokenOnDestroy()).Forget();
        }

        #endregion Class Methdos
    }
}