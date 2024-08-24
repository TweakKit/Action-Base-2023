using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class PullToCenterEffectModel : StatusEffectModel
    {
        private readonly float _pullSpeed;
        private readonly float _duration;

        public override StatusEffectType StatusEffectType => StatusEffectType.PullToCenter;
        public override bool IsStackable => false;
        public override bool IsOneShot => true;

        public PullToCenterEffectModel(float pullSpeed, float duration, float chance = 1f) : base(chance)
        {
            _pullSpeed = pullSpeed;
            _duration = duration;
        }

        public float GetPullSpeed()
        {
            return _pullSpeed;
        }

        public float GetDuration()
        {
            return _duration;
        }

        public override StatusEffectModel Clone()
            => MemberwiseClone() as PullToCenterEffectModel;
    }
}