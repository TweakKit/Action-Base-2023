using UnityEngine;

namespace Runtime.Manager.Pool
{
    public class AutoReturnPool : ReturnPool
    {
        #region Members

        [SerializeField]
        [Min(0.001f)]
        protected float returnDelayTime;

        [SerializeField]
        [Tooltip("If ticked, the particle time is the delay time.")]
        protected bool isReturnByParticleTime;

        #endregion Members

        #region Properties

        protected override float ReturnDelayTime
        {
            get
            {
                if (isReturnByParticleTime)
                {
                    float particleTime = 0.0f;
                    var particleSystems = transform.GetComponentsInChildren<ParticleSystem>();

                    foreach (ParticleSystem particleSystem in particleSystems)
                        if (particleSystem.main.startLifetime.constantMax > particleTime)
                            particleTime = particleSystem.main.startLifetime.constantMax * (1 / particleSystem.main.simulationSpeed);

                    return particleTime;
                }

                return returnDelayTime;
            }
        }

        #endregion Properties
    }
}