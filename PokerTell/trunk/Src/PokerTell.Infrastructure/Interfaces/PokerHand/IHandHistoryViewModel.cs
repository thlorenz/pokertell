namespace PokerTell.Infrastructure.Interfaces.PokerHand
{
    using System;
    using System.Collections.Generic;

    public interface IHandHistoryViewModel
    {
        string Ante { get; }

        string BigBlind { get; }

        string GameId { get; }

        string SmallBlind { get; }

        string TimeStamp { get; }

        string TotalPlayers { get; }

        string TournamentId { get; }

        bool IsTournament { get; }

        bool HasAnte { get; }

        IBoardViewModel Board { get; }

        Action<IPokerHandCondition> AdjustToConditionAction { get; }

        string[] Note { get; set; }

        IList<IHandHistoryRow> PlayerRows { get; }

        bool Visible { get; set; }

        IHandHistoryViewModel UpdateWith(IConvertedPokerHand hand);

        IHandHistoryViewModel Initialize(bool showPreflopFolds);
    }
}