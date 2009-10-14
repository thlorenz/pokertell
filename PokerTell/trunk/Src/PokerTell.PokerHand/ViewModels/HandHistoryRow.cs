namespace PokerTell.PokerHand.ViewModels
{
    using System.Text;

    using PokerTell.Infrastructure.Enumerations.PokerHand;
    using PokerTell.Infrastructure.Interfaces.PokerHand;

    using Tools.GenericUtilities;

    public class HandHistoryRow : IHandHistoryRow
    {
        #region Constants and Fields

        const string Indent = "___     ";

        readonly IConvertedPokerPlayer _pokerPlayer;

        #endregion

        #region Constructors and Destructors

        public HandHistoryRow(IConvertedPokerPlayer pokerPlayer)
        {
            _pokerPlayer = pokerPlayer;
        }

        #endregion

        #region Properties

        public string Flop
        {
            get { return GetActionsFor(Streets.Flop); }
        }

        public IHoleCardsViewModel HoleCards
        {
            get
            {
                var hcvm = new HoleCardsViewModel();
                hcvm.UpdateWith(_pokerPlayer.Holecards);
                return hcvm;
            }
        }

        public string M
        {
            get { return _pokerPlayer.MBefore.ToString(); }
        }

        public string PlayerName
        {
            get { return _pokerPlayer.Name; }
        }

        public string Position
        {
            get { return _pokerPlayer.StrategicPosition.ToString(); }
        }

        public string Preflop
        {
            get
            {
                return _pokerPlayer.StrategicPosition == StrategicPositions.SB ||
                       _pokerPlayer.StrategicPosition == StrategicPositions.BB
                           ? Indent + GetActionsFor(Streets.PreFlop)
                           : GetActionsFor(Streets.PreFlop);
            }
        }

        public string River
        {
            get { return GetActionsFor(Streets.River); }
        }

        public string Turn
        {
            get { return GetActionsFor(Streets.Turn); }
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Member.State(() => Position) + "  :  ");
            sb.Append(Member.State(() => PlayerName) + "  :  ");
            sb.Append(Member.State(() => Preflop) + "  :  ");
            sb.Append(Member.State(() => Flop) + "  :  ");
            sb.Append(Member.State(() => Turn) + "  :  ");
            sb.Append(Member.State(() => River) + "  :  ");
            return sb.ToString();
        }

        #endregion

        #region Methods

        string GetActionsFor(Streets street)
        {
            return _pokerPlayer.Count > (int)street && _pokerPlayer[street] != null
                       ? _pokerPlayer[street].ToString()
                       : string.Empty;
        }

        #endregion
    }
}