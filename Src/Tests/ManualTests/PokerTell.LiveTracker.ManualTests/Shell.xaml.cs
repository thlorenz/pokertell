namespace PokerTell.LiveTracker.ManualTests
{
    using System.Windows;
    using System.Windows.Input;

    using Infrastructure;

    using Tools;

    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell
    {
        readonly IShellViewModel _viewModel;

        public Shell()
        {
            InitializeComponent();

            if (Utils.OperatingSystemIsWindowsXPOrOlder())
            {
                Background = ApplicationProperties.BorderedWindowBackgoundBrush;
                AllowsTransparency = false;
            }
        }

        public Shell(IShellViewModel shellViewModel)
            : this()
        {
            _viewModel = shellViewModel;
            DataContext = _viewModel;
        }

        void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void WindowBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}