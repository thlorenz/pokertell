namespace PokerTell.Plugins.PlayerPeeker
{
    using System;
    using System.Reflection;

    using log4net;

    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Composite.Presentation.Commands;
    using Microsoft.Practices.Composite.Regions;
    using Microsoft.Practices.Unity;

    using PokerTell.Infrastructure;
    using PokerTell.Plugins.PlayerPeeker.Interfaces;

    [ModuleDependency(ApplicationModules.UserModule)]
    public class PlayerPeekerModule : IModule
    {
        static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        readonly IUnityContainer _container;

        readonly IRegionManager _regionManager;

        public PlayerPeekerModule(IUnityContainer container, IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _container = container;
        }

        public void Initialize()
        {
            _container
                .RegisterTypeAndConstructor<IPlayerPeekerForm, PlayerPeekerForm>(() => _container.Resolve<IPlayerPeekerForm>());

            GlobalCommands.StartServicesCommand.RegisterCommand(new DelegateCommand<object>(StartServices));
          
            Log.Info("got initialized");
        }

        void StartServices(object ignore)
        {
            var playerPeekerService = _container.Resolve<PlayerPeekerService>();
            _regionManager.RegisterViewWithRegion(ApplicationProperties.TableOverlayToolBarRegion, () => playerPeekerService.ShowPlayerPeekerButton);

            Log.Info("started services");
        }
    }
}