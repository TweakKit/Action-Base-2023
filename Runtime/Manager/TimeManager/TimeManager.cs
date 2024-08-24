using System;
using System.Collections.Generic;
using UnityEngine;
using Runtime.UI;
using Runtime.Common.Singleton;
using Runtime.FeatureSystem;
using Runtime.Manager.Data;
using Runtime.Message;
using Runtime.Definition;
using Runtime.PlayerManager;
using Runtime.Manager.Network;
using Cysharp.Threading.Tasks;

namespace Runtime.Manager.Time
{
    public sealed class TimeManager : MonoSingleton<TimeManager>
    {
        #region Members

        [SerializeField]
        private TimeCounter _dayResetTimeCounter;
        [SerializeField]
        private TimeCounter _weekResetTimeCounter;
        [SerializeField]
        private TimeCounter _refreshGachaTimeCounter;
        private Dictionary<int, TimeCounter> _flashSaleTimeCounterDictionary = new();
        private Dictionary<int, TimeCounter> _segmentPackTimeCounterDictionary = new();
        
        #endregion Members

        #region Properties

        public static long NowInSeconds
        {
            get
            {
                return (long)((Time - Constant.JAN1St1970).TotalSeconds);
            }
        }

        public static DateTime Time
        {
            get
            {
#if UNITY_EDITOR
                return DateTime.UtcNow;
#endif
                if (HasServerTime)
                    return TimeOnTimeServerGot.AddSeconds(UnityEngine.Time.realtimeSinceStartup - RealTimeOnTimeServerGot);
                else
                    return DateTime.UtcNow.AddSeconds(NetworkChecker.ServerTimeDeltaInSeconds);
            }
        }

        public static double TotalElapsedTimeSinceStartUp
        {
            get
            {
                var elapsedTime = Time - StartUpTime;
                return elapsedTime.TotalSeconds;
            }
        }

        public static DateTime TimeOnTimeServerGot { get; set; }
        public static float RealTimeOnTimeServerGot { get; set; }
        public static bool HasServerTime { get; set; }
        private static DateTime StartUpTime { get; set; } = DateTime.UtcNow;
        private static bool HasSetStartUpTime { get; set; }
        private bool HasStarted { get; set; }
        private bool HasStartedCheckingOnApplicationResponse { get; set; }

        #endregion Properties

        #region API Methods

#if UNITY_EDITOR
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!HasStarted)
                return;

            if (hasFocus)
                RunOnApplicationResponseAsync().Forget();
        }
#else
        private void OnApplicationPause(bool isPause)
        {
            if (!HasStarted)
                return;

            if (!isPause)
                RunOnApplicationResponseAsync().Forget();
        }
#endif

        #endregion API Methods

        #region Class Methods

        public static DateTime ConverToDateTime(long secondsFromOriginTime)
        {
            var timeSpan = TimeSpan.FromSeconds(secondsFromOriginTime);
            return Constant.JAN1St1970 + timeSpan;
        }

        public void HandleDataLoaded()
        {
            HasStarted = true;
            StartTickAsync().Forget();
        }

        public void StartLoadFlashSaleTime(int uniquePackId)
        {
            if (_flashSaleTimeCounterDictionary.ContainsKey(uniquePackId))
            {
                if (_flashSaleTimeCounterDictionary[uniquePackId] != null)
                {
                    var timeRemain = DataManager.Local.GetFlashSaleRemainingTime(uniquePackId);
                    _flashSaleTimeCounterDictionary[uniquePackId].Cancel();
                    _flashSaleTimeCounterDictionary[uniquePackId].Tick(NowInSeconds + timeRemain, onFinishTimeRemain: () => OnFinishedFlashSaleTimeCounter(uniquePackId));
                }
            }
        }

        public void AddNewTimeCounterFlashSale(int uniquePackId, TimeCounter timeCounter)
        {
            if (!_flashSaleTimeCounterDictionary.ContainsKey(uniquePackId))
                _flashSaleTimeCounterDictionary.Add(uniquePackId, timeCounter);
            else
                _flashSaleTimeCounterDictionary[uniquePackId] = timeCounter;
        }
        
        public void StartLoadSegmentPackTime(int uniquePackId)
        {
            if (_segmentPackTimeCounterDictionary.ContainsKey(uniquePackId))
            {
                if (_segmentPackTimeCounterDictionary[uniquePackId] != null)
                {
                    var timeRemain = DataManager.Local.GetFlashSaleRemainingTime(uniquePackId);
                    _segmentPackTimeCounterDictionary[uniquePackId].Cancel();
                    _segmentPackTimeCounterDictionary[uniquePackId].Tick(NowInSeconds + timeRemain, onFinishTimeRemain: () => OnFinishedSegmentPackTimeCounter(uniquePackId));
                }
            }
        }

        public void AddNewTimeCounterSegmentPack(int uniquePackId, TimeCounter timeCounter)
        {
            if (!_segmentPackTimeCounterDictionary.ContainsKey(uniquePackId))
                _segmentPackTimeCounterDictionary.Add(uniquePackId, timeCounter);
            else
                _segmentPackTimeCounterDictionary[uniquePackId] = timeCounter;
        }

        public void StartLoadRefreshGachaTime()
        {
            var timeRemain = DataManager.Config.TimeResetRefreshGacha;
            DataManager.Local.SaveTimeRefreshGachaStart();
            _refreshGachaTimeCounter.Cancel();
            _refreshGachaTimeCounter.Tick(NowInSeconds + timeRemain, onFinishTimeRemain: OnFinishedStartRefreshGachaTimeCounter);
        }

        public void TickRefreshGachaTimeCounter()
        {
            var timeRemain = DataManager.Local.GetRefreshGachaRemainingTime();
            _refreshGachaTimeCounter.Cancel();
            _refreshGachaTimeCounter.Tick(NowInSeconds + timeRemain, onFinishTimeRemain: OnFinishedRefreshGachaTimeCounter);
        }

        public void CancelLoadTimeFlashSale(int uniquePackId)
        {
            if (_flashSaleTimeCounterDictionary[uniquePackId] != null)
                _flashSaleTimeCounterDictionary[uniquePackId].Dispose();
        }
        
        public void CancelLoadTimeSegment(int uniquePackId)
        {
            if (_segmentPackTimeCounterDictionary[uniquePackId] != null)
                _segmentPackTimeCounterDictionary[uniquePackId].Dispose();
        }

        private async UniTask RunOnApplicationResponseAsync()
        {
            if (!HasStartedCheckingOnApplicationResponse)
            {
                HasStartedCheckingOnApplicationResponse = true;
                await NetworkChecker.WaitForConnectionAsync(this.GetCancellationTokenOnDestroy());
                Singleton.Of<PlayerService>().CheckResetDaily();
                Singleton.Of<PlayerService>().CheckResetWeekly();
                HasStartedCheckingOnApplicationResponse = false;
            }
        }

        private async UniTask StartTickAsync()
        {
            await NetworkChecker.WaitForConnectionAsync(this.GetCancellationTokenOnDestroy());
            if (!HasSetStartUpTime)
            {
                HasSetStartUpTime = true;
                StartUpTime = Time;
            }
            Singleton.Of<PlayerService>().CheckResetDaily();
            Singleton.Of<PlayerService>().CheckResetWeekly();
            InitDayResetTimeCounter();
            InitWeekResetTimeCounter();
            CheckTickRefreshGachaTimeCounter();
        }

        private void InitDayResetTimeCounter()
        {
            var timeRemain = Constant.TIME_OF_A_DAY_IN_SECONDS - NowInSeconds % Constant.TIME_OF_A_DAY_IN_SECONDS;
            _dayResetTimeCounter.Cancel();
            _dayResetTimeCounter.Tick(NowInSeconds + timeRemain, onFinishTimeRemain: OnFinishedDayResetTimeCounter);
        }

        private void InitWeekResetTimeCounter()
        {
            var timeRemain = Constant.TIME_OF_A_WEEK_IN_SECONDS - NowInSeconds % Constant.TIME_OF_A_WEEK_IN_SECONDS;
            _weekResetTimeCounter.Cancel();
            _weekResetTimeCounter.Tick(NowInSeconds + timeRemain, onFinishTimeRemain: OnFinishedWeekResetTimeCounter);
        }

        private void OnFinishedDayResetTimeCounter()
            => CheckFinishDayResetTimeCounterAsync().Forget();

        private void OnFinishedWeekResetTimeCounter()
            => CheckFinishWeekResetTimeCounterAsync().Forget();

        private async UniTask CheckFinishDayResetTimeCounterAsync()
        {
            await CheckHasServerTime();
            Singleton.Of<PlayerService>().CheckResetDaily();
            InitDayResetTimeCounter();
        }

        private async UniTask CheckFinishWeekResetTimeCounterAsync()
        {
            await CheckHasServerTime();
            Singleton.Of<PlayerService>().CheckResetWeekly();
            InitWeekResetTimeCounter();
        }

        public void CheckTickFlashSaleTimeCounter()
        {
            for (int i = 0; i < Constant.LIST_STORE_PRODUCT_TYPE_FLASH_SALE_LV.Count; i++)
            {
                var uniquePackId = Constant.LIST_STORE_PRODUCT_TYPE_FLASH_SALE_LV[i];
                if (FeatureUnlockChecker.IsFlashSaleUnlocked(uniquePackId))
                {
                    if (DataManager.Local.IsFlashSaleAvaiable(uniquePackId))
                        TickFlashSaleTimeCounter(uniquePackId);
                }
            }
        }
        
        public void CheckTickSegmentPackTimeCounter()
        {
            for (int i = 0; i < Constant.LIST_SEGMENT_UINIQUE_ID.Count; i++)
            {
                var uniquePackId = Constant.LIST_SEGMENT_UINIQUE_ID[i];
                if (FeatureUnlockChecker.IsSegmentPackUnlocked(uniquePackId))
                {
                    if (DataManager.Local.IsSegmentPackAvaiable(uniquePackId))
                        TickSegmentPackTimeCounter(uniquePackId);
                }
            }
        }

        private void CheckTickRefreshGachaTimeCounter()
        {
            if (FeatureUnlockChecker.IsGachaGateUnlocked())
            {
                if (DataManager.Local.IsRefreshGachaTimeRemainCompleted())
                {
                    if (HasServerTime)
                    {
                        DataManager.Local.SaveTimeRefreshGachaStart();
                        DataManager.Transitioned.IsAvailableRefreshGacha = true;
                        Messenger.Publish(new DataUpdatedMessage(DataUpdatedType.GachaRefresh));
                        Messenger.Publish(new GameStateChangedMessage(GameStateEventType.RefreshGachaCompleted));
                    }
                }
                TickRefreshGachaTimeCounter();
            }
        }

        private void TickFlashSaleTimeCounter(int uniquePackId)
        {
            var timeRemain = DataManager.Local.GetFlashSaleRemainingTime(uniquePackId);
            _flashSaleTimeCounterDictionary[uniquePackId].Cancel();
            _flashSaleTimeCounterDictionary[uniquePackId].Tick(NowInSeconds + timeRemain, onFinishTimeRemain: () => OnFinishedFlashSaleTimeCounter(uniquePackId));
        }
        
        private void TickSegmentPackTimeCounter(int uniquePackId)
        {
            var timeRemain = DataManager.Local.GetSegmentPackRemainingTime(uniquePackId);
            _segmentPackTimeCounterDictionary[uniquePackId].Cancel();
            _segmentPackTimeCounterDictionary[uniquePackId].Tick(NowInSeconds + timeRemain, onFinishTimeRemain: () => OnFinishedSegmentPackTimeCounter(uniquePackId));
        }

        private void OnFinishedFlashSaleTimeCounter(int uniquePackId)
            => Messenger.Publish(new EndFlashSaleMessage(uniquePackId));
        
        private void OnFinishedSegmentPackTimeCounter(int uniquePackId)
            => Messenger.Publish(new EndSegmentPackMessage(uniquePackId));

        private void OnFinishedRefreshGachaTimeCounter()
            => CheckFinishRefreshGachaTimeCounterAsync().Forget();

        private async UniTask CheckFinishRefreshGachaTimeCounterAsync()
        {
            await CheckHasServerTime();
            if (DataManager.Local.IsRefreshGachaTimeRemainCompleted())
            {
                DataManager.Local.SaveTimeRefreshGachaStart();
                if (!ScreenNavigator.Instance.IsShowingModal(ModalId.GACHA_HERO))
                    DataManager.Transitioned.IsAvailableRefreshGacha = true;
                else
                    TickRefreshGachaTimeCounter();
                Messenger.Publish(new DataUpdatedMessage(DataUpdatedType.GachaRefresh));
                Messenger.Publish(new GameStateChangedMessage(GameStateEventType.RefreshGachaCompleted));
            }
        }

        private void OnFinishedStartRefreshGachaTimeCounter()
            => CheckFinishStartRefreshGachaTimeCounterAsync().Forget();

        private async UniTask CheckFinishStartRefreshGachaTimeCounterAsync()
        {
            await CheckHasServerTime();
            if (DataManager.Local.IsRefreshGachaTimeRemainCompleted())
            {
                if (!ScreenNavigator.Instance.IsShowingModal(ModalId.GACHA_HERO))
                    DataManager.Transitioned.IsAvailableRefreshGacha = true;
                else
                    StartLoadRefreshGachaTime();
                Messenger.Publish(new DataUpdatedMessage(DataUpdatedType.GachaRefresh));
                Messenger.Publish(new GameStateChangedMessage(GameStateEventType.RefreshGachaCompleted));
            }
        }

        private async UniTask CheckHasServerTime()
        {
            if (HasServerTime)
                await UniTask.CompletedTask;
            else
                await NetworkChecker.WaitForConnectionAsync(this.GetCancellationTokenOnDestroy());
        }

        #endregion Class Methods
    }
}