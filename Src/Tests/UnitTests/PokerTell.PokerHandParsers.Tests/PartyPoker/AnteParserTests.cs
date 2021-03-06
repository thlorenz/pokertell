namespace PokerTell.PokerHandParsers.Tests.PartyPoker
{
    using PokerTell.PokerHandParsers.Base;

    public class AnteParserTests : Base.AnteParserTests
    {
        protected override AnteParser GetAnteParser()
        {
            return new PokerTell.PokerHandParsers.PartyPoker.AnteParser();
        }

        protected override string ValidTournamentAnte(double ante)
        {
            // Blinds-Antes(40.000/80.000 -2.000)
            // still need to account for the "." as the thousand delimiter
            return string.Format("Blinds-Antes(40.000/80.000 -{0})", ante);
        }
    }
}