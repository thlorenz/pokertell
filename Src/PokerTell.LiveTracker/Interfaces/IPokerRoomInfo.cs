namespace PokerTell.LiveTracker.Interfaces
{
    using Infrastructure.Interfaces;

    public interface IPokerRoomInfo : IFluentInterface
    {
        string Site { get; }

        string TableClass { get; }

        string ProcessName { get; }

        string FileExtension { get; }

        IPokerRoomDetective Detective { get; }

        string HelpWithHandHistoryDirectorySetupLink { get; }

        string TableNameFoundInPokerTableTitleFrom(string parsedName);
    }
}