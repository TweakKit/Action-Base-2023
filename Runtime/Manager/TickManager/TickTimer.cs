using System;

namespace Runtime.Manager.Tick
{
    public class TickTimer
    {
        #region Members

        public int id;
        public float rate;
        public float currentRate;
        public Action tickCompletedCallbackAction;
        public Action<int> tickTimeFinishedTickAction;

        #endregion Members

        #region Class Methods

        public TickTimer(int id, float rate, Action tickCompletedCallbackAction, Action<int> tickTimeFinishedTickAction)
        {
            this.id = id;
            this.rate = rate < 0 ? 0 : rate;
            this.tickCompletedCallbackAction = tickCompletedCallbackAction;
            this.tickTimeFinishedTickAction = tickTimeFinishedTickAction;
            currentRate = 0;
        }

        public void Tick(float deltaTime)
        {
            currentRate += deltaTime;
            if (currentRate >= rate)
                InstantlyDone();
        }

        public float GetCurrentPercent()
        {
            return currentRate / this.rate;
        }

        public void InstantlyDone()
        {
            currentRate = 0;
            tickCompletedCallbackAction.Invoke();
            tickTimeFinishedTickAction.Invoke(id);
        }

        public void Cancel()
        {
            currentRate = 0;
            tickTimeFinishedTickAction.Invoke(id);
        }

        #endregion Class Methods
    }
}