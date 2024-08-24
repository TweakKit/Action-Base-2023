using Runtime.Message;
using Runtime.Definition;
using Runtime.Common.Singleton;
using UnityEngineTime = UnityEngine.Time;

namespace Runtime.Manager.Game
{
    public partial class GameManager : PersistentMonoSingleton<GameManager>
    {
        #region Members

        private int _gameFlowStoppedSourcesCount;
        private GameMomentType _gameMomentType;

        #endregion Members

        #region Properties

        public GameMomentType GameMomentType => _gameMomentType;
        public bool IsGameOver => _gameMomentType == GameMomentType.WonGame ||
                                  _gameMomentType == GameMomentType.LostGame;
        public bool IsInGameplay => _gameMomentType == GameMomentType.StartGame;

        #endregion Properties

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            _gameMomentType = GameMomentType.None;
            _gameFlowStoppedSourcesCount = 0;
        }

        #endregion API Methods

        #region Class Methods

        public void StopGameFlow()
        {
            Messenger.Publish(new GameStateChangedMessage(GameStateEventType.GameFlowStopped));
            _gameFlowStoppedSourcesCount++;
            if (!IsGameOver)
                UnityEngineTime.timeScale = 0.0f;
        }

        public void ContinueGameFlow()
        {
            _gameFlowStoppedSourcesCount--;
            if (_gameFlowStoppedSourcesCount <= 0)
            {
                _gameFlowStoppedSourcesCount = 0;
                if (!IsGameOver)
                    UnityEngineTime.timeScale = 1.0f;
            }
        }

        public void SetGameMomentType(GameMomentType gameMomentType)
        {
            _gameMomentType = gameMomentType;
            if (_gameMomentType == GameMomentType.StartGame)
                UnityEngineTime.timeScale = 1.0f;
        }

        #endregion Class Methods
    }
}