﻿namespace PokerTell.SessionReview.Views
{
    using System.Windows.Controls;

    using ViewModels;

    /// <summary>
    /// Interaction logic for SessionReviewSettingsView.xaml
    /// </summary>
    public partial class SessionReviewSettingsView 
    {
        #region Constructors and Destructors

        public SessionReviewSettingsView(SessionReviewSettingsViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;
            DataContext = viewModel;
        }

        public SessionReviewSettingsViewModel ViewModel { get; private set; }

        #endregion

        private void All_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.ShowAll = true;
            }
        }

        private void MoneyInvested_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.ShowMoneyInvested = true;
            }
        }

        private void SawFlop_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.ShowSawFlop = true;
            }
        }

        private void SelectedOnly_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.ShowSelectedOnly = true;
            }
        }
    }
}