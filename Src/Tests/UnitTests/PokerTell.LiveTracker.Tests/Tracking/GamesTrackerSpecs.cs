namespace PokerTell.LiveTracker.Tests.Tracking
{
    using System;
    using System.Collections.Generic;

    using Machine.Specifications;

    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Presentation.Events;

    using Moq;

    using PokerTell.Infrastructure.Events;
    using PokerTell.Infrastructure.Interfaces;
    using PokerTell.Infrastructure.Interfaces.PokerHand;
    using PokerTell.Infrastructure.Services;
    using PokerTell.LiveTracker.Events;
    using PokerTell.LiveTracker.Interfaces;
    using PokerTell.LiveTracker.Tracking;

    using It = Machine.Specifications.It;

    // Resharper disable InconsistentNaming
    public abstract class GamesTrackerSpecs
    {
        protected static IEventAggregator _eventAggregator;

        protected static Mock<IGameController> _gameController_Mock;

        protected static Mock<IConstructor<IHandHistoryFilesWatcher>> _filesWatcherMake_Mock;

        protected static Mock<ILiveTrackerSettingsViewModel> _settings_Stub;

        protected static Mock<IWatchedDirectoriesOptimizer> _watchedDirectoriesOptimizer_Mock;

        protected static Mock<INewHandsTracker> _newHandsTracker_Mock;

        protected static GamesTrackerSut _sut;

        Establish specContext = () => {
            _eventAggregator = new EventAggregator();
            _gameController_Mock = new Mock<IGameController>();
            _filesWatcherMake_Mock = new Mock<IConstructor<IHandHistoryFilesWatcher>>();

            _newHandsTracker_Mock = new Mock<INewHandsTracker>();

            _settings_Stub = new Mock<ILiveTrackerSettingsViewModel>();
            _settings_Stub
                .SetupGet(ls => ls.HandHistoryFilesPaths).Returns(new string[] { });

            _watchedDirectoriesOptimizer_Mock = new Mock<IWatchedDirectoriesOptimizer>();
            _watchedDirectoriesOptimizer_Mock
                .Setup(wo => wo.Optimize(Moq.It.IsAny<IList<string>>())).Returns(new string[] { });

            _filesWatcherMake_Mock
                .SetupGet(fwm => fwm.New).Returns(new Mock<IHandHistoryFilesWatcher>().Object);

            _sut = new GamesTrackerSut(_eventAggregator,
                                       _watchedDirectoriesOptimizer_Mock.Object,
                                       _newHandsTracker_Mock.Object,
                                       new Constructor<IGameController>(() => _gameController_Mock.Object),
                                       _filesWatcherMake_Mock.Object)
                { ThreadOption = ThreadOption.PublisherThread };
        };

        public abstract class Ctx_InitializedWithLiveTrackerSettings : GamesTrackerSpecs
        {
            Establish initializedContext = () => _sut.InitializeWith(_settings_Stub.Object);
        }

        [Subject(typeof(GamesTracker), "InitializeWith")]
        public class when_initialized_with_live_tracker_settings : GamesTrackerSpecs
        {
            static Mock<ILiveTrackerSettingsViewModel> newSettings_Stub;

            Establish context = () => {
                newSettings_Stub = new Mock<ILiveTrackerSettingsViewModel>();
                newSettings_Stub
                    .SetupGet(ls => ls.HandHistoryFilesPaths).Returns(new string[] { });

                _sut.AdjustedToNewLiveTrackerSettings = false;
            };

            Because of = () => _sut.InitializeWith(newSettings_Stub.Object);

            It should_set_the_LiveTrackerSettings_to_the_ones_that_were_passed = () => _sut._LiveTrackerSettings.ShouldBeTheSameAs(newSettings_Stub.Object);

            It should_adjust_to_the_new_live_tracker_settings = () => _sut.AdjustedToNewLiveTrackerSettings.ShouldBeTrue();
        }

        [Subject(typeof(GamesTracker), "LiveTrackerSettings changed")]
        public class when_the_LiveTrackerSettings_changed : Ctx_InitializedWithLiveTrackerSettings
        {
            static Mock<ILiveTrackerSettingsViewModel> newSettings_Stub;

            Establish context = () => {
                newSettings_Stub = new Mock<ILiveTrackerSettingsViewModel>();
                newSettings_Stub
                    .SetupGet(ls => ls.HandHistoryFilesPaths).Returns(new string[] { });

                _sut.AdjustedToNewLiveTrackerSettings = false;
            };

            Because of = () => _eventAggregator
                                   .GetEvent<LiveTrackerSettingsChangedEvent>()
                                   .Publish(newSettings_Stub.Object);

            It should_set_the_LiveTrackerSettings_to_the_ones_that_were_passed = () => _sut._LiveTrackerSettings.ShouldBeTheSameAs(newSettings_Stub.Object);

            It should_adjust_to_the_new_live_tracker_settings = () => _sut.AdjustedToNewLiveTrackerSettings.ShouldBeTrue();
        }


        [Subject(typeof(GamesTracker), "StartTracking")]
        public class when_told_to_start_tracking_a_file_that_is_tracked_already : Ctx_InitializedWithLiveTrackerSettings
        {
            const string directory = @"c:\someDir\";

            const string filename = "someFile.txt";

            const string fullPath = directory + filename;

            static string userMessage;

            Establish context = () => {
                _sut.GameControllers.Add(fullPath, new Mock<IGameController>().Object);

                _eventAggregator
                    .GetEvent<UserMessageEvent>()
                    .Subscribe(args => userMessage = args.UserMessage);
            };

            Because of = () => _sut.StartTracking(fullPath);

            It should_not_add_the_file_to_the_GameControllers = () => _sut.GameControllers.Count.ShouldEqual(1);

            It should_raise_a_user_message_containing_the_filename = () => userMessage.ShouldContain("\"" + filename + "\"");
        }

        [Subject(typeof(GamesTracker), "StartTracking")]
        public class when_told_to_start_tracking_a_file_that_is_not_tracked_yet : Ctx_InitializedWithLiveTrackerSettings
        {
            const string fullPath = "somePath";

            Because of = () => _sut.StartTracking(fullPath);

            It should_add_a_GameController_for_the_full_path_of_the_file = () => _sut.GameControllers.Keys.ShouldContain(fullPath);

            It should_set_the_GameController_LiveTracker_Settings_to_the_ones_passed_in
                = () => _gameController_Mock.VerifySet(gc => gc.LiveTrackerSettings = _settings_Stub.Object);

            It should_tell_the_NewHandsTracker_to_process_the_file = () => _newHandsTracker_Mock.Verify(ht => ht.ProcessHandHistoriesInFile(fullPath));
        }

        [Subject(typeof(GamesTracker), "New Hand found")]
        public class when_told_that_a_new_hand_for_a_file_that_it_is_tracking_was_found : Ctx_InitializedWithLiveTrackerSettings
        {
            const string fullPath = "somePath";

            static Mock<IConvertedPokerHand> newHand_Stub;

            Establish context = () => {
                newHand_Stub = new Mock<IConvertedPokerHand>();
                _sut.GameControllers.Add(fullPath, _gameController_Mock.Object);
            };

            Because of = () => _eventAggregator
                                   .GetEvent<NewHandEvent>()
                                   .Publish(new NewHandEventArgs(fullPath, newHand_Stub.Object));

            It should_tell_the_GameController_about_the_new_hand = () => _gameController_Mock.Verify(gc => gc.NewHand(newHand_Stub.Object));
        }

        [Subject(typeof(GamesTracker), "New Hand found")]
        public class when_a_new_hand_was_found_and_it_is_currently_tracking_no_files_and_it_is_supposed_to_AutoTrack :
            Ctx_InitializedWithLiveTrackerSettings
        {
            const string fullPath = "somePath";

            static Mock<IConvertedPokerHand> newHand_Stub;

            Establish context = () => {
                newHand_Stub = new Mock<IConvertedPokerHand>();
                _settings_Stub.SetupGet(s => s.AutoTrack).Returns(true);
            };

            Because of = () => _eventAggregator
                                   .GetEvent<NewHandEvent>()
                                   .Publish(new NewHandEventArgs(fullPath, newHand_Stub.Object));

            It should_add_a_new_GameController_for_the_full_path = () => _sut.GameControllers.Keys.ShouldContain(fullPath);

            It should_set_the_GameController_LiveTracker_Settings_to_the_ones_passed_in
                = () => _gameController_Mock.VerifySet(gc => gc.LiveTrackerSettings = _settings_Stub.Object);

            It should_tell_the_GameController_about_the_new_hand = () => _gameController_Mock.Verify(gc => gc.NewHand(newHand_Stub.Object));
        }

        [Subject(typeof(GamesTracker), "New Hand found")]
        public class when_a_new_hand_was_found_and_it_is_currently_tracking_no_files_but_it_is_not_supposed_to_AutoTrack :
            Ctx_InitializedWithLiveTrackerSettings
        {
            const string fullPath = "somePath";

            static Mock<IConvertedPokerHand> newHand_Stub;

            Establish context = () => {
                newHand_Stub = new Mock<IConvertedPokerHand>();
                _settings_Stub.SetupGet(s => s.AutoTrack).Returns(false);
            };

            Because of = () => _eventAggregator
                                   .GetEvent<NewHandEvent>()
                                   .Publish(new NewHandEventArgs(fullPath, newHand_Stub.Object));

            It should_not_add_a_new_GameController_for_the_full_path = () => _sut.GameControllers.Keys.ShouldNotContain(fullPath);
        }

        [Subject(typeof(GamesTracker), "Adjust to new live tracker settings")]
        public class when_told_to_adjust_ot_livetracker_settings_with_two_new_paths_when_tracking_one_path_already : GamesTrackerSpecs
        {
            const string trackedPath = "tracked Path";
            
            const string firstPath = "firstPath";

            const string secondPath = "secondPath";

            static IList<string> settingsPaths;

            Establish context = () => {
                settingsPaths = new[] { firstPath, secondPath };
                _settings_Stub
                    .SetupGet(ls => ls.HandHistoryFilesPaths).Returns(settingsPaths);
                _watchedDirectoriesOptimizer_Mock
                    .Setup(wo => wo.Optimize(Moq.It.IsAny<IList<string>>())).Returns(new[] { firstPath });
                _sut
                    .HandHistoryFilesWatchers.Add(trackedPath, new Mock<IHandHistoryFilesWatcher>().Object);
            };

            Because of = () => _sut.AdjustToNewLiveTrackerSettings_Invoke(_settings_Stub.Object);

            It should_ask_the_optimizer_to_optimize_the_tracked_paths_combined_with_the_paths_contained_in_the_settings
                = () => _watchedDirectoriesOptimizer_Mock.Verify(wo => wo.Optimize(
                    Moq.It.Is<IList<string>>(paths => paths.Contains(trackedPath) && paths.Contains(firstPath) && paths.Contains(secondPath))));

            It should_only_create_watchers_for_the_path_returned_by_the_optimizer = () => _sut.HandHistoryFilesWatchers.Keys.ShouldContainOnly(firstPath);
        }

        [Subject(typeof(GamesTracker), "Adjust to new LiveTrackerSettings")]
        public class when_told_to_adjust_to_live_tracker_settings_with_two_distinct_paths_with_different_roots : GamesTrackerSpecs
        {
            const string firstPath = "firstPath";

            const string secondPath = "secondPath";

            static Mock<IHandHistoryFilesWatcher> firstFilesWatcher;

            static Mock<IHandHistoryFilesWatcher> secondFilesWatcher;

            static string[] thePaths;
            
            Establish context = () => {
                thePaths = new[] { firstPath, secondPath };
                _settings_Stub
                    .SetupGet(ls => ls.HandHistoryFilesPaths).Returns(thePaths);
                _watchedDirectoriesOptimizer_Mock
                    .Setup(wo => wo.Optimize(Moq.It.IsAny<IList<string>>())).Returns(thePaths);

                firstFilesWatcher = new Mock<IHandHistoryFilesWatcher>();
                secondFilesWatcher = new Mock<IHandHistoryFilesWatcher>();
                firstFilesWatcher
                    .Setup(fw => fw.InitializeWith(firstPath)).Returns(firstFilesWatcher.Object);
                secondFilesWatcher
                    .Setup(fw => fw.InitializeWith(secondPath)).Returns(secondFilesWatcher.Object);

                int resolved = 0;
                Func<IHandHistoryFilesWatcher> resolveFilesWatcher = () => resolved++ == 0 ? firstFilesWatcher.Object : secondFilesWatcher.Object;
                _filesWatcherMake_Mock
                    .SetupGet(wm => wm.New).Returns(resolveFilesWatcher);
            };

            Because of = () => _sut.AdjustToNewLiveTrackerSettings_Invoke(_settings_Stub.Object);

            It should_create_a_hand_history_files_watcher_for_each_hand_history_files_path
                = () => _filesWatcherMake_Mock.VerifyGet(make => make.New, Times.Exactly(2));

            It should_initialize_the_first_watcher_with_the_first_path
                = () => firstFilesWatcher.Verify(fw => fw.InitializeWith(firstPath));

            It should_initialize_the_second_watcher_with_the_second_path
                = () => secondFilesWatcher.Verify(fw => fw.InitializeWith(secondPath));

            It should_initialize_the_NewHandsTracker_with_the_HandHistory_files_watchers
                = () => _newHandsTracker_Mock.Verify(ht => ht.InitializeWith(_sut.HandHistoryFilesWatchers.Values));

        }

        [Subject(typeof(GamesTracker), "Adjust to new LiveTrackerSettings")]
        public class when_the_live_tracker_settings_changed_and_it_contains_one_GameController : GamesTrackerSpecs
        {
            Establish context = () => {
                _sut.GameControllers.Add("somePath", _gameController_Mock.Object);
                _settings_Stub
                    .SetupGet(ls => ls.HandHistoryFilesPaths).Returns(new string[] { });
            };

            Because of = () => _sut.AdjustToNewLiveTrackerSettings_Invoke(_settings_Stub.Object);

            It should_set_the_GameController_LiveTracker_Settings_to_the_new_ones
                = () => _gameController_Mock.VerifySet(gc => gc.LiveTrackerSettings = _settings_Stub.Object);

            It should_set_the_LiveTrackersettings_to_the_new_ones = () => _sut._LiveTrackerSettings.ShouldEqual(_settings_Stub.Object);
        }

        [Subject(typeof(GamesTracker), "Adjust to new LiveTrackerSettings")]
        public class
            when_live_tracker_settings_changed_and_hand_history_files_paths_contain_on_path_that_is_not_tracked_and_one_previously_tracked_path_was_removed :
                Ctx_InitializedWithLiveTrackerSettings
        {
            const string firstPath = "firstPath";

            const string secondPath = "secondPath";

            const string thirdPath = "thirdPath";

            static Mock<IHandHistoryFilesWatcher> firstFilesWatcher;

            static Mock<IHandHistoryFilesWatcher> secondFilesWatcher;

            static Mock<IHandHistoryFilesWatcher> thirdFilesWatcher;

            static Mock<ILiveTrackerSettingsViewModel> newLiveTrackerSettings;

            Establish context = () => {
                firstFilesWatcher = new Mock<IHandHistoryFilesWatcher>();
                secondFilesWatcher = new Mock<IHandHistoryFilesWatcher>();
                thirdFilesWatcher = new Mock<IHandHistoryFilesWatcher>();

                _sut.HandHistoryFilesWatchers.Add(firstPath, firstFilesWatcher.Object);
                _sut.HandHistoryFilesWatchers.Add(secondPath, secondFilesWatcher.Object);

                _filesWatcherMake_Mock
                    .SetupGet(wm => wm.New).Returns(thirdFilesWatcher.Object);

                newLiveTrackerSettings = new Mock<ILiveTrackerSettingsViewModel>();
                newLiveTrackerSettings
                    .SetupGet(ls => ls.HandHistoryFilesPaths).Returns(new[] { firstPath, thirdPath });
            };

            Because of = () => _sut.AdjustToNewLiveTrackerSettings_Invoke(newLiveTrackerSettings.Object);

            It should_keep_the_path_that_was_previously_tracked_and_is_also_contained_in_the_current_HandHistoryPaths
                = () => _sut.HandHistoryFilesWatchers.Keys.ShouldContain(firstPath);

            It should_remove_the_path_that_was_previously_tracked_but_is_not_contained_in_the_current_HandHistoryPaths
                = () => _sut.HandHistoryFilesWatchers.Keys.ShouldNotContain(secondPath);

            It should_dispose_the_filetracker_for_the_path_that_it_is_removing = () => secondFilesWatcher.Verify(fw => fw.Dispose());

            It should_add_the_path_that_was_not_tracked_before_but_was_contained_in_the_current_HandHistoryPaths
                = () => _sut.HandHistoryFilesWatchers.Keys.ShouldContain(thirdPath);

            It should_initialize_the_newly_added_files_watcher = () => thirdFilesWatcher.Verify(fw => fw.InitializeWith(thirdPath));

            It should_reinitialize_the_NewHandsTracker_with_the_current_HandHistoryFilesWatchers
                = () => _newHandsTracker_Mock.Verify(ht => ht.InitializeWith(_sut.HandHistoryFilesWatchers.Values));

        }

        [Subject(typeof(GamesTracker), "Adjust to new LiveTrackerSettings")]
        public class when_LiveTrackerSettings_changed_but_the_tracked_paths_did_not_change : Ctx_InitializedWithLiveTrackerSettings
        {
            const string firstPath = "firstPath";

            static Mock<IHandHistoryFilesWatcher> firstFilesWatcher;

            Establish context = () => {
                firstFilesWatcher = new Mock<IHandHistoryFilesWatcher>();

                _sut.HandHistoryFilesWatchers.Add(firstPath, firstFilesWatcher.Object);

                var newLiveTrackerSettings = new Mock<ILiveTrackerSettingsViewModel>();
                newLiveTrackerSettings
                    .SetupGet(ls => ls.HandHistoryFilesPaths).Returns(new[] { firstPath });
            };

            Because of = () => _sut.AdjustToNewLiveTrackerSettings_Invoke(_settings_Stub.Object);

            It should_not_reinitialize_the_NewHandsTracker_with_the_current_HandHistoryFilesWatchers
                = () => _newHandsTracker_Mock.Verify(ht => ht.InitializeWith(_sut.HandHistoryFilesWatchers.Values));
        }

        [Subject(typeof(GamesTracker), "GameController shuts down")]
        public class when_a_GameController_signals_shut_down : Ctx_InitializedWithLiveTrackerSettings
        {
            const string fullPath = "somePath";

            Establish context = () => _sut.StartTracking(fullPath);

            Because of = () => _gameController_Mock.Raise(gc => gc.ShuttingDown += null);

            It should_remove_it_from_the_GameControllers = () => _sut.GameControllers.Values.ShouldNotContain(_gameController_Mock.Object);
        }
    }

    public class GamesTrackerSut : GamesTracker
    {
        public GamesTrackerSut(
            IEventAggregator eventAggregator, 
            IWatchedDirectoriesOptimizer watchedDirectoriesOptimizer, 
            INewHandsTracker newHandsTracker, 
            IConstructor<IGameController> gameControllerMake, 
            IConstructor<IHandHistoryFilesWatcher> handHistoryFilesWatcherMake)
            : base(eventAggregator, watchedDirectoriesOptimizer, newHandsTracker, gameControllerMake, handHistoryFilesWatcherMake)
        {
        }

        public bool AdjustedToNewLiveTrackerSettings { get; set; }

        internal ILiveTrackerSettingsViewModel _LiveTrackerSettings
        {
            get { return _liveTrackerSettings; }
        }

        internal void AdjustToNewLiveTrackerSettings_Invoke(ILiveTrackerSettingsViewModel updatedLiveTrackerSettings)
        {
            AdjustToNewLiveTrackerSettings(updatedLiveTrackerSettings);
        }

        protected override void AdjustToNewLiveTrackerSettings(ILiveTrackerSettingsViewModel updatedLiveTrackerSettings)
        {
            base.AdjustToNewLiveTrackerSettings(updatedLiveTrackerSettings);
            AdjustedToNewLiveTrackerSettings = true;
        }
  }
}