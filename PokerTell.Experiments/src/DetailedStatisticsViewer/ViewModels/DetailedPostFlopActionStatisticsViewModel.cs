namespace DetailedStatisticsViewer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Input;

    using Tools.WPF;

    public class DetailedPostFlopActionStatisticsViewModel : DetailedStatisticsViewModel
    {
        #region Constants and Fields

        ICommand _investigateCommand;

        #endregion

        #region Constructors and Destructors

        public DetailedPostFlopActionStatisticsViewModel()
            : base("Bet Size")
        {
            Rows = new List<IRowViewModel>
                {
                    new RowViewModel("Bet", new[] { 20, 30, 12, 40, 30, 12 }, "%"),
                    new RowViewModel("Count", new[] { 1, 4, 30, 34, 33, 30, 12 }, string.Empty)
                };
        }

        #endregion

        #region Properties

        public ICommand InvestigateCommand
        {
            get
            {
                return _investigateCommand ?? (_investigateCommand = new SimpleCommand
                    {
                        ExecuteDelegate = _ =>
                        {
                            var sb = new StringBuilder();
                            sb.AppendLine("Investigating: ");
                            SelectedCells.ToList().ForEach(cell => sb.Append(cell + ", "));
                            Console.WriteLine(sb);
                        },
                        CanExecuteDelegate = _ => SelectedCells.Count() > 0
                    });
            }
        }

        #endregion
    }
}