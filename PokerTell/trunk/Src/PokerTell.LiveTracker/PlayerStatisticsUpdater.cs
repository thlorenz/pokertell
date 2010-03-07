namespace PokerTell.LiveTracker
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    using log4net;

    using PokerTell.Infrastructure.Interfaces.Statistics;
    using PokerTell.LiveTracker.Interfaces;

    using Tools.FunctionalCSharp;

    public class PlayerStatisticsUpdater : IPlayerStatisticsUpdater
    {
        static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        readonly BackgroundWorker _backgroundWorker;

        public PlayerStatisticsUpdater()
        {
            _backgroundWorker = new BackgroundWorker();

            _backgroundWorker.DoWork += (_, e) => {
                var playerStatistics = (IEnumerable<IPlayerStatistics>) e.Argument;
                playerStatistics.ForEach(ps => ps.UpdateStatistics());
                e.Result = playerStatistics;
            };

            _backgroundWorker.RunWorkerCompleted += (_, e) => {
                var updatedPlayerStatistics = (IEnumerable<IPlayerStatistics>) e.Result;
                FinishedUpdatingPlayerStatistics(updatedPlayerStatistics);
            };
        }

        public void Update(IEnumerable<IPlayerStatistics> playerStatistics)
        {
            if (playerStatistics == null)
            {
                Log.Debug("Passed playerStatistics were null -> couldn't update");
                return;
            }

            if (_backgroundWorker.IsBusy) return; 

            _backgroundWorker.RunWorkerAsync(playerStatistics);
        }

        public event Action<IEnumerable<IPlayerStatistics>> FinishedUpdatingPlayerStatistics;
    }
}