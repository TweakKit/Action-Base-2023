using System;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Common.Singleton;

namespace Runtime.Manager.Tick
{
    public class TickManager : MonoSingleton<TickManager>
    {
        #region Members

        private List<TickTimer> _tickTimers = new List<TickTimer>();
        private List<int> _removalPending = new List<int>();
        private int _unownedTickIdCount;

        #endregion Members

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            _unownedTickIdCount = -1;
        }

        private void Update()
        {
            Remove();
            Tick();
        }

        #endregion API Methods

        #region Class Methods

        public TickTimer AddOwnedTimer(uint ownedTickId, float rate, Action tickCompletedCallbackAction)
        {
            var newTimer = new TickTimer((int)ownedTickId, rate, tickCompletedCallbackAction, RemoveTimer);
            _tickTimers.Add(newTimer);
            return newTimer;
        }

        public int AddUnownedTimer(float rate, Action tickCompletedCallbackAction)
        {
            var tickId = --_unownedTickIdCount;
            var newTimer = new TickTimer(tickId, rate, tickCompletedCallbackAction, RemoveTimer);
            _tickTimers.Add(newTimer);
            return tickId;
        }

        public void RemoveOwnedTimer(uint ownedTickId)
        {
            var tickId = (int)ownedTickId;
            RemoveTimer(tickId);
        }

        public void RemoveUnownedTimer(int unownedTickId)
            => RemoveTimer(unownedTickId);

        public void InstantlyDone(uint ownedTickId)
        {
            for (int i = 0; i < _tickTimers.Count; i++)
            {
                if (_tickTimers[i].id == ownedTickId)
                {
                    _tickTimers[i].InstantlyDone();
                    break;
                }
            }
        }

        public void Cancel(uint ownedTickId)
        {
            for (int i = 0; i < _tickTimers.Count; i++)
            {
                if (_tickTimers[i].id == ownedTickId)
                {
                    _tickTimers[i].Cancel();
                    break;
                }
            }
        }

        private void RemoveTimer(int timerId)
        {
            if (!_removalPending.Contains(timerId))
                _removalPending.Add(timerId);
        }

        private void Remove()
        {
            if (_removalPending.Count > 0)
            {
                foreach (int id in _removalPending)
                {
                    for (int i = 0; i < _tickTimers.Count; i++)
                    {
                        if (_tickTimers[i].id == id)
                        {
                            _tickTimers.RemoveAt(i);
                            break;
                        }
                    }
                }
                _removalPending.Clear();
            }
        }

        private void Tick()
        {
            for (int i = 0; i < _tickTimers.Count; i++)
                this._tickTimers[i].Tick(UnityEngine.Time.deltaTime);
        }

        #endregion Class Methods
    }
}