using System;
using System.Threading;
using Runtime.Manager.Time;
using Runtime.Common.Singleton;
using Runtime.Manager.Location;
using Runtime.Localization;
using Runtime.Manager.Toast;
using Cysharp.Threading.Tasks;
using Runtime.SceneLoading;

namespace Runtime.Manager.Network
{
    public class NetworkChecker : PersistentMonoSingleton<NetworkChecker>
    {
        #region Members

        public float connectionTimeout;
        public int delayToReconnectMillisecond;
        private long _serverTimeDeltaInSeconds;
        private bool _hasStartedFetching;
        private bool _hasDoneFetching;
        private bool _connectionFetchedResult;

        #endregion Members

        #region Properties

        public static long ServerTimeDeltaInSeconds
            => Instance._serverTimeDeltaInSeconds;

        #endregion Properties

        #region Class Methods

        public static async UniTask RunCheckConnectionWithCallbackAsync(Action actionCallback, CancellationToken cancellationToken)
        {
            SceneManager.ShowSceneDataLoading();
            var hasConnection = await Instance.WaitForUpdateDeltaTimeFromServerAsync(cancellationToken);
            if (hasConnection)
                actionCallback?.Invoke();
            else
                ToastManager.Instance.Show(LocalizationUtils.GetToastLocalized(LocalizeKeys.NO_INTERNET), ToastVisualType.Text);
            SceneManager.HideSceneDataLoading();
        }

        public static async UniTask<bool> RunCheckConnectionAsync(CancellationToken cancellationToken)
        {
            var hasConnection = await Instance.WaitForUpdateDeltaTimeFromServerAsync(cancellationToken);
            return hasConnection;
        }

        public static async UniTask WaitForConnectionAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                var hasConnection = await Instance.WaitForUpdateDeltaTimeFromServerAsync(cancellationToken);
                if (hasConnection)
                    break;
                else
                    await UniTask.Delay(Instance.delayToReconnectMillisecond, true, cancellationToken: cancellationToken);
            }
        }

        private async UniTask<bool> WaitForUpdateDeltaTimeFromServerAsync(CancellationToken cancellationToken)
        {
            if (!_hasStartedFetching)
            {
                _hasStartedFetching = true;
                _hasDoneFetching = false;
                var locationManager = Singleton.Of<LocationManager>();
                var locationInBound = await locationManager.FetchAsync(connectionTimeout, cancellationToken);
                if (locationInBound != null)
                {
                    var clientTimeInMs = DateTime.UtcNow.TimeInMilliseconds();
                    var delta = (locationInBound.CurrentTimeInMillisecond - clientTimeInMs) / 1000;
                    _serverTimeDeltaInSeconds = delta;
                    TimeManager.HasServerTime = true;
                    TimeManager.TimeOnTimeServerGot = DateTime.UtcNow.AddSeconds(_serverTimeDeltaInSeconds);
                    TimeManager.RealTimeOnTimeServerGot = UnityEngine.Time.realtimeSinceStartup;
                }
                _connectionFetchedResult = locationInBound != null;
                _hasDoneFetching = true;
                _hasStartedFetching = false;
                return _connectionFetchedResult;
            }
            else
            {
                await UniTask.WaitUntil(() => _hasDoneFetching, PlayerLoopTiming.Update, cancellationToken: cancellationToken);
                return _connectionFetchedResult;
            }
        }

        #endregion Class Methods
    }
}