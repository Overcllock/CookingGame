using Cysharp.Threading.Tasks;
using Game.AssetsManagement;
using Game.Campaign;
using Game.Content;
using Game.Events;
using Game.Infrastructure;
using Game.Presenters;
using Game.UI;
using Game.UI.Windows;
using UnityEngine;

namespace Game.Main
{
    public class GameInstaller : MonoBehaviour
    {
        //NOTE: ONLY FOR TEST TASK! This is so bad, ui containers must be managed not here, UI layers system needed
        [SerializeField]
        private Transform _mainCanvas;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            var serviceLocatorManager = new SimpleServiceLocatorManager();
            ServiceLocator.Initialize(serviceLocatorManager);
            
            UnityEventsManager eventsManager = new UnityEventsManager();
            EngineEvents.Initialize(eventsManager);
            
            UnityAssetsManager assetsManager = new UnityAssetsManager();
            AssetsLoader.Initialize(assetsManager);

            Initialize().Forget();
        }

        private async UniTaskVoid Initialize()
        {
            LoadContent();
            await UniTask.WaitWhile(() => !ContentManager.isLoaded);

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            
            InitializeServices();
        }

        private void InitializeServices()
        {
            ServiceLocator.Add(this);
            
            ServiceLocator.Add(new UILayoutFactory(_mainCanvas));
            ServiceLocator.Add(new UIWindowsDispatcher(new UIWindowsManager()));

            ServiceLocator.Add(new CampaignService());

            var gameLoop = new GameLoopService();
            ServiceLocator.Add(gameLoop);
            
            ServiceLocator.Add(new CampaignPresenter());
            
            gameLoop.TrySwitchTo(GameMode.TitleScreen);
        }

        private void LoadContent()
        {
            var instantiator = new ContentInstantiator();
            var configurer = new RuntimeContentConfigurer(instantiator);
            var system = new ContentManagementSystem(configurer);
            
            ContentManager.Initialize(system);
        }

        private void OnApplicationQuit()
        {
            ServiceLocator.Dispose();
        }
    }
}