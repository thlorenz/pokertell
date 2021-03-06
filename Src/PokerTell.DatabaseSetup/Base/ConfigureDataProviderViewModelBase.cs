namespace PokerTell.DatabaseSetup.Base
{
    using System;
    using System.Reflection;
    using System.Windows.Input;

    using Infrastructure.Events;
    using Infrastructure.Interfaces.DatabaseSetup;

    using log4net;

    using Microsoft.Practices.Composite.Events;

    using Properties;

    using Tools.WPF;
    using Tools.WPF.ViewModels;

    public abstract class ConfigureDataProviderViewModelBase : NotifyPropertyChanged
    {
        protected readonly IEventAggregator _eventAggregator;

        protected string _serverConnectString;

        static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        readonly IDatabaseConnector _databaseConnector;

        readonly IDatabaseSettings _databaseSettings;

        ICommand _cancelCommand;

        bool _connectionIsValid;

        string _password;

        ICommand _saveCommand;

        string _serverName;

        ICommand _setDefaultsCommand;

        ICommand _testConnectionCommand;

        string _userName;

        protected ConfigureDataProviderViewModelBase(
            IEventAggregator eventAggregator, 
            IDatabaseSettings databaseSettings, 
            IDatabaseConnector databaseConnector)
        {
            _databaseConnector = databaseConnector;
            _eventAggregator = eventAggregator;
            _databaseSettings = databaseSettings;
        }

        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new SimpleCommand
                    {
                        ExecuteDelegate = arg => Console.WriteLine("Cancelling")
                    });
            }
        }

        public string Password
        {
            get { return _password ?? string.Empty; }
            set
            {
                _password = value;
                _connectionIsValid = false;
                RaisePropertyChanged(() => Password);
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new SimpleCommand
                    {
                        ExecuteDelegate =
                            arg => {
                                _databaseSettings.SetServerConnectStringFor(DataProviderInfo, _serverConnectString);
                                ShowUserMessageSettingsSaved();
                            }, 
                        CanExecuteDelegate = arg => _connectionIsValid
                    });
            }
        }

        public string ServerName
        {
            get { return _serverName ?? string.Empty; }
            set
            {
                _serverName = value;
                _connectionIsValid = false;
                RaisePropertyChanged(() => ServerName);
            }
        }

        public ICommand SetDefaultsCommand
        {
            get
            {
                return _setDefaultsCommand ?? (_setDefaultsCommand = new SimpleCommand
                    {
                        ExecuteDelegate = arg => InitializeWithDefaults(), 
                    });
            }
        }

        public ICommand TestConnectionCommand
        {
            get
            {
                return _testConnectionCommand ?? (_testConnectionCommand = new SimpleCommand
                    {
                        ExecuteDelegate = arg => TestConnection(), 
                        CanExecuteDelegate =
                            arg =>
                            ! (string.IsNullOrEmpty(ServerName) || string.IsNullOrEmpty(UserName) || Password == null)
                    });
            }
        }

        public string UserName
        {
            get { return _userName ?? string.Empty; }
            set
            {
                _userName = value;
                _connectionIsValid = false;
                RaisePropertyChanged(() => UserName);
            }
        }

        protected abstract IDataProviderInfo DataProviderInfo { get; }

        public ConfigureDataProviderViewModelBase Initialize()
        {
            _databaseConnector.InitializeWith(DataProviderInfo);
            try
            {
                if (_databaseSettings.ProviderIsAvailable(DataProviderInfo))
                {
                    var serverConnectInfo =
                        new DatabaseConnectionInfo(_databaseSettings.GetServerConnectStringFor(DataProviderInfo));
                    return InititializeWith(
                        serverConnectInfo.Server, serverConnectInfo.User, serverConnectInfo.Password);
                }
            }
            catch (Exception excep)
            {
                Log.Error(excep);
            }

            return InitializeWithDefaults();
        }

        protected ConfigureDataProviderViewModelBase InititializeWith(string serverName, string userName, string password)
        {
            ServerName = serverName;
            UserName = userName;
            Password = password;
            return this;
        }

        ConfigureDataProviderViewModelBase InitializeWithDefaults()
        {
            return InititializeWith("localhost", "root", string.Empty);
        }

        void ShowUserMessageSettingsSaved()
        {
            var userMessage = new UserMessageEventArgs(
                UserMessageTypes.Info, Resources.Info_SettingsSaved);
            _eventAggregator.GetEvent<UserMessageEvent>().Publish(userMessage);
        }

        void TestConnection()
        {
            _serverConnectString = new DatabaseConnectionInfo(ServerName, UserName, Password).ServerConnectString;
            _databaseConnector.TryToConnectToServerUsing(_serverConnectString, true);

            _connectionIsValid = _databaseConnector.DataProvider != null && _databaseConnector.DataProvider.IsConnectedToServer;
        }
    }
}