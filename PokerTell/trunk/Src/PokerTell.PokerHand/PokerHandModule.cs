namespace PokerTell.PokerHand
{
    using System;
    using System.Reflection;

    using log4net;

    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Composite.Regions;
    using Microsoft.Practices.Unity;

    using PokerTell.Infrastructure.Interfaces.PokerHand;
    using PokerTell.Infrastructure.Services;
    using PokerTell.PokerHand.Analyzation;
    using PokerTell.PokerHand.Aquisition;
    using PokerTell.PokerHand.ViewModels;
    using PokerTell.PokerHand.Views;

    using HandHistoryViewModel = PokerTell.PokerHand.ViewModels.Design.HandHistoryViewModel;

    public class PokerHandModule : IModule
    {
        #region Constants and Fields

        static readonly ILog Log = LogManager.GetLogger(
            MethodBase.GetCurrentMethod().DeclaringType);

        readonly IUnityContainer _container;

        readonly IRegionManager _regionManager;

        #endregion

        #region Constructors and Destructors

        public PokerHandModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        #endregion

        #region Implemented Interfaces

        #region IModule

        public void Initialize()
        {
            RegisterViewsAndServices();
            try
            {
                _regionManager.RegisterViewWithRegion(
                    "Shell.MainRegion", 
                    () => _container.Resolve<HandHistoryView>());
                _regionManager.Regions["Shell.MainRegion"].Add(_container.Resolve<HandHistoryView>());
            }
            catch (Exception excep)
            {
                Log.Error("Resolving", excep);
            }

            // _regionManager.Regions["Shell.MainRegion"].Add(
            // new HandHistoryView(new HandHistoryViewModel()));
            Log.Info("Initialized PokerHandModule");
        }

        #endregion

        #endregion

        #region Methods

        protected void RegisterViewsAndServices()
        {
            _container
                .RegisterConstructor<IAquiredPokerAction, AquiredPokerAction>()
                .RegisterConstructor<IAquiredPokerRound, AquiredPokerRound>()
                .RegisterConstructor<IAquiredPokerPlayer, AquiredPokerPlayer>()
                .RegisterConstructor<IAquiredPokerHand, AquiredPokerHand>()
                .RegisterConstructor<IConvertedPokerAction, ConvertedPokerAction>()
                .RegisterConstructor<IConvertedPokerActionWithId, ConvertedPokerActionWithId>()
                .RegisterConstructor<IConvertedPokerRound, ConvertedPokerRound>()
                .RegisterConstructor<IConvertedPokerPlayer, ConvertedPokerPlayer>()
                .RegisterConstructor<IConvertedPokerHand, ConvertedPokerHand>()
            
                .RegisterType<IHoleCardsViewModel, HoleCardsViewModel>()
                .RegisterType<IBoardViewModel, BoardViewModel>()
                .RegisterType<IHandHistoryViewModel, HandHistoryViewModel>();
        }

        #endregion
    }
}