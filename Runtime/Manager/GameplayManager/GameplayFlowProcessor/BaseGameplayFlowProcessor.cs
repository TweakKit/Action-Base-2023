using System.Threading;
using UnityEngine;
using Runtime.Message;
using Runtime.Definition;
using Runtime.SceneLoading;
using Runtime.Gameplay.Map;
using Runtime.Manager.Game;
using Runtime.Manager.Proximity;
using Runtime.Manager.QuestManager;
using Runtime.Common.Singleton;
using Runtime.Gameplay.EntitySystem;

namespace Runtime.Gameplay.Manager
{
    public abstract class BaseGameplayFlowProcessor<EM, GM, MM> : MonoBehaviour where EM : EntitiesManager
                                                                                where GM : GameResourceManager
                                                                                where MM : MapManager

    {
        #region Members

        protected EM entitiesManager;
        protected GM gameResourceManager;
        protected MM mapManager;
        protected HeroesControlManager heroesControllerManager;
        protected CameraManager cameraManager;
        protected ProximityManager proximityManager;
        protected NavigationManager navigationManager;
        protected QuestManager questManager;
        protected CancellationTokenSource gameplayFlowCancellationTokenSource;
        protected MessageRegistry<HeroDiedMessage> heroDiedMessageRegistry;
        protected MessageRegistry<EnemyDiedMessage> enemyDiedMessageRegistry;
        protected MessageRegistry<ObjectDestroyedMessage> objectDestroyedMessageRegistry;
        protected MessageRegistry<GameStateChangedMessage> gameStateChangedMessageRegistry;
        protected MessageRegistry<GateEventTriggeredMessage> gateEventTriggeredMessageRegistry;
        protected MessageRegistry<GameActionNotifiedMessage> gameActionNotifiedMessageRegistry;
        protected MessageRegistry<HeroesControllingStatusUpdatedMessage> heroControllingStatusUpdatedMessageRegistry;

        #endregion Members

        #region API Methods

        protected virtual void Awake()
        {
            entitiesManager = EntitiesManager.Instance as EM;
            gameResourceManager = GameResourceManager.Instance as GM;
            mapManager = MapManager.Instance as MM;
            heroesControllerManager = HeroesControlManager.Instance;
            cameraManager = CameraManager.Instance;
            proximityManager = ProximityManager.Instance;
            navigationManager = NavigationManager.Instance;
            questManager = Singleton.Of<QuestManager>();
            gameplayFlowCancellationTokenSource = new CancellationTokenSource();
            heroDiedMessageRegistry = Messenger.Subscribe<HeroDiedMessage>(OnHeroDied);
            enemyDiedMessageRegistry = Messenger.Subscribe<EnemyDiedMessage>(OnEnemyDied);
            objectDestroyedMessageRegistry = Messenger.Subscribe<ObjectDestroyedMessage>(OnObjectDestroyed);
            gameStateChangedMessageRegistry = Messenger.Subscribe<GameStateChangedMessage>(OnGameStateChanged);
            gateEventTriggeredMessageRegistry = Messenger.Subscribe<GateEventTriggeredMessage>(OnGateEventTriggered);
            gameActionNotifiedMessageRegistry = Messenger.Subscribe<GameActionNotifiedMessage>(OnGameActionNotified);
            heroControllingStatusUpdatedMessageRegistry = Messenger.Subscribe<HeroesControllingStatusUpdatedMessage>(OnHeroesControllingStatusUpdated);
            SceneManager.RegisterBeforeChangeScene(OnBeforeChangeScene);
        }

        protected virtual void OnDestroy()
        {
            gameplayFlowCancellationTokenSource.Cancel();
            gameplayFlowCancellationTokenSource.Dispose();
            heroDiedMessageRegistry.Dispose();
            enemyDiedMessageRegistry.Dispose();
            objectDestroyedMessageRegistry.Dispose();
            gameStateChangedMessageRegistry.Dispose();
            gateEventTriggeredMessageRegistry.Dispose();
            gameActionNotifiedMessageRegistry.Dispose();
            heroControllingStatusUpdatedMessageRegistry.Dispose();
        }

        #endregion API Methods

        #region Class Methods

        protected virtual void OnHeroDied(HeroDiedMessage heroDiedMessage)
        {
            var playResult = PlayResult.None;
            entitiesManager.HandleHeroDied(heroDiedMessage, gameplayFlowCancellationTokenSource.Token, out playResult);
            navigationManager.HandleHeroDied();
            cameraManager.HandleHeroDied();
            proximityManager.HandleHeroDied();
            heroesControllerManager.HandleHeroDied();
            switch (playResult)
            {
                case PlayResult.LostGame:
                    HandleGameLost();
                    break;
            }
        }

        protected virtual void OnEnemyDied(EnemyDiedMessage enemyDiedMessage)
        {
            entitiesManager.HandleEnemyDied(enemyDiedMessage, gameplayFlowCancellationTokenSource.Token);
            gameResourceManager.HandleEnemyDied(enemyDiedMessage, gameplayFlowCancellationTokenSource.Token);
            mapManager.HandleEnemyDied(enemyDiedMessage, gameplayFlowCancellationTokenSource.Token);
            if (CanAllowQuest(enemyDiedMessage.EnemyModel))
            {
                Messenger.Publish(new GameActionNotifiedMessage(GameActionType.KillEnemy, enemyDiedMessage.EnemyModel.EntityId, 1));
                Messenger.Publish(new GameActionNotifiedMessage(GameActionType.Hunt, enemyDiedMessage.EnemyModel.EntityId, 1));
                if (enemyDiedMessage.EnemyModel.IsElite)
                    Messenger.Publish(new GameActionNotifiedMessage(GameActionType.HuntElite, enemyDiedMessage.EnemyModel.EntityId, 1));
            }
        }

        protected virtual bool CanAllowQuest(EnemyModel enemyModel) => !enemyModel.IgnoreQuest;

        protected virtual void OnObjectDestroyed(ObjectDestroyedMessage objectDestroyedMessage)
        {
            entitiesManager.HandleObjectDestroyed(objectDestroyedMessage, gameplayFlowCancellationTokenSource.Token);
            gameResourceManager.HandleObjectDestroyed(objectDestroyedMessage, gameplayFlowCancellationTokenSource.Token);
        }

        protected virtual void OnGameStateChanged(GameStateChangedMessage gameStateChangedMessage)
        {
            switch (gameStateChangedMessage.GameStateEventType)
            {
                case GameStateEventType.GameFlowStopped:
                    heroesControllerManager.HandleGameFlowStopped();
                    break;

                case GameStateEventType.DataLoaded:
                    GameManager.Instance.SetGameMomentType(GameMomentType.StartGame);
                    heroesControllerManager.HandleDataLoaded(gameplayFlowCancellationTokenSource.Token);
                    cameraManager.HandleDataLoaded(heroesControllerManager.HeroesGroupCenterHolder);
                    mapManager.HandleDataLoaded(gameplayFlowCancellationTokenSource.Token);
                    InitQuestOnHandleDataLoaded();
                    break;

                case GameStateEventType.HeroSpawned:
                    heroesControllerManager.HandleHeroesSpawned();
                    navigationManager.HandleHeroesSpawned();
                    proximityManager.HandleHeroesSpawned();
                    cameraManager.HandleHeroesSpawned();
                    break;
            }
        }

        protected virtual void OnGateEventTriggered(GateEventTriggeredMessage gateEventTriggeredMessage)
            => mapManager.HandleGateEventTriggered(gateEventTriggeredMessage);

        protected virtual void OnGameActionNotified(GameActionNotifiedMessage gameActionNotifiedMessage)
            => questManager.HandleQuestChanged(gameActionNotifiedMessage);

        protected virtual void OnHeroesControllingStatusUpdated(HeroesControllingStatusUpdatedMessage heroesControllingStatusUpdatedMessage)
            => cameraManager.HandleHeroesControlingStatusUpdated(heroesControllingStatusUpdatedMessage);

        protected virtual void HandleGameLost()
        {
            GameManager.Instance.SetGameMomentType(GameMomentType.LostGame);
            Messenger.Publish(new GameStateChangedMessage(GameStateEventType.GameLost));
        }

        protected virtual void InitQuestOnHandleDataLoaded()
            => questManager.HandleDataLoaded();

        protected virtual void OnBeforeChangeScene()
        {
            var disposableEntities = FindObjectsOfType<Disposable>(true);
            foreach (var disposableEntity in disposableEntities)
                disposableEntity.Dispose();
        }

        #endregion Class Methods
    }
}