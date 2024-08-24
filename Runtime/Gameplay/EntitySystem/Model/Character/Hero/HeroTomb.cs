using System;
using Runtime.Manager.Tick;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class HeroTomb : MonoBehaviour
    {
        private TickTimer _timer;
        private bool isInitialized;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private GameObject _timerVisual;

        private void Awake()
        {
            _spriteRenderer.material.SetFloat("_Angle", 270f);
        }

        public void Init(TickTimer tickTimer)
        {
            this._timer = tickTimer;
            _timerVisual.gameObject.SetActive(true);
            isInitialized = true;
        }

        private void Update()
        {
            if (isInitialized)
            {
                float current = 1 - _timer.GetCurrentPercent();
                _spriteRenderer.material.SetFloat("_Arc1", (current) * 360f);
                if (current <= 0)
                {
                    Hide();
                }
            }
        }

        private void Hide()
        {
            _timerVisual.SetActive(false);
            isInitialized = false;
        }
    }
}