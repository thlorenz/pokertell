namespace PokerTell.Statistics.ViewModels.Analyzation
{
   using System;
   using System.Collections.Generic;
   using System.Linq;

   using PokerTell.Infrastructure.Interfaces.PokerHand;
   using PokerTell.Infrastructure.Interfaces.Statistics;
   using PokerTell.Statistics.Interfaces;

   using Tools.WPF.ViewModels;

   public class RepositoryHandBrowserViewModel : NotifyPropertyChanged, IRepositoryHandBrowserViewModel
   {
      readonly IRepositoryHandBrowser _repositoryHandBrowser;

      int _currentHandIndex;

      public RepositoryHandBrowserViewModel(IRepositoryHandBrowser repositoryHandBrowser, IHandHistoryViewModel handHistoryViewModel)
      {
         CurrentHandHistory = handHistoryViewModel;
         _repositoryHandBrowser = repositoryHandBrowser;
      }

      // This event does not occur here because this ViewModel the last in the chain
      public event Action<IDetailedStatisticsAnalyzerContentViewModel> ChildViewModelChanged = delegate { };

       public bool ShowAsPopup
       {
        get { return false; }
       }

      public IHandHistoryViewModel CurrentHandHistory { get; protected set; }

      public int HandCount { get; protected set; }

      public int CurrentHandIndex
      {
         get { return _currentHandIndex; }
         set {
            _currentHandIndex = value;
            CurrentHandHistory.UpdateWith(_repositoryHandBrowser.Hand(_currentHandIndex)); 
            RaisePropertyChanged(() => CurrentHandIndex);
         }
      }

      public IRepositoryHandBrowserViewModel InitializeWith(IEnumerable<IAnalyzablePokerPlayer> analyzablePokerPlayers)
      {
         var reversedHandIds = analyzablePokerPlayers.Select(p => p.HandId).Reverse();

         _repositoryHandBrowser.InitializeWith(reversedHandIds);

          CurrentHandIndex = 0;
          HandCount = _repositoryHandBrowser.PotentialHandsCount;
          return this;
      }
   }
}