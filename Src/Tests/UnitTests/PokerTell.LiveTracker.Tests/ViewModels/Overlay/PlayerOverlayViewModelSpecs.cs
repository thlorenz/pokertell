namespace PokerTell.LiveTracker.Tests.ViewModels.Overlay
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    using Machine.Specifications;

    using Moq;

    using PokerTell.Infrastructure.Interfaces.PokerHand;
    using PokerTell.Infrastructure.Interfaces.Statistics;
    using PokerTell.LiveTracker.Interfaces;
    using PokerTell.LiveTracker.ViewModels.Overlay;

    using Tools.WPF.Interfaces;
    using Tools.WPF.ViewModels;

    using It = Machine.Specifications.It;

    // Resharper disable InconsistentNaming
    public abstract class PlayerOverlayViewModelSpecs
    {
        protected static Mock<ITableOverlaySettingsViewModel> _overlaySettings_Stub;

        protected static Mock<IPlayerStatusViewModel> _playerStatusVM_Mock;

        protected static Mock<IPlayerStatisticsViewModel> _playerStatisticsVM_Stub;

        protected static IPlayerOverlayViewModel _sut;

        protected const int SeatNumber = 0;

        Establish specContext = () => {
            _overlaySettings_Stub = new Mock<ITableOverlaySettingsViewModel>();
            _playerStatusVM_Mock = new Mock<IPlayerStatusViewModel>();
            _playerStatisticsVM_Stub = new Mock<IPlayerStatisticsViewModel>();

            _sut = new PlayerOverlayViewModel(_playerStatusVM_Mock.Object);
        };

        public abstract class Ctx_initialized_with_settings_and_seat_number_0 : PlayerOverlayViewModelSpecs
        {
            protected const double left = 1.0;

            protected const double top = 2.0;

            Establish context = () => {
                _overlaySettings_Stub.SetupGet(os => os.PlayerStatisticsPanelPositions).Returns(new[] { new PositionViewModel(left, top) });
                _sut.InitializeWith(_overlaySettings_Stub.Object, SeatNumber);
            };
        }

        public abstract class Ctx_NonNull_ConvertedPlayer_Statistics_And_Setup_HarringtonM : PlayerOverlayViewModelSpecs
        {
            protected static Mock<IConvertedPokerPlayer> _convertedPlayer_Stub;

            protected static Mock<IPlayerStatisticsViewModel> _playerStatistics_Stub;

            protected static Mock<IHarringtonMViewModel> _harrintonM_VM_Stub;

            Establish context = () => {
                _convertedPlayer_Stub = new Mock<IConvertedPokerPlayer>();
                _playerStatistics_Stub = new Mock<IPlayerStatisticsViewModel>();
                _harrintonM_VM_Stub = new Mock<IHarringtonMViewModel>();
                _playerStatusVM_Mock.SetupGet(ps => ps.HarringtonM).Returns(_harrintonM_VM_Stub.Object);
                _playerStatusVM_Mock.SetupGet(ps => ps.IsPresent).Returns(true);
            };
        }

        [Subject(typeof(PlayerOverlayViewModel), "InitializeWith")]
        public class when_initialized_with_settings_and_seat_1 : PlayerOverlayViewModelSpecs
        {
            static IList<IPositionViewModel> harringtonMPositions;

            static IList<IPositionViewModel> holeCardsPositions;

            const int seat = 1;

            Establish context = () => {
                harringtonMPositions = new[] { null, new PositionViewModel(1, 1) };
                holeCardsPositions = new[] { null, new PositionViewModel(2, 2) };
                _overlaySettings_Stub.SetupGet(os => os.PlayerStatisticsPanelPositions).Returns(new IPositionViewModel[] { null, null });
                _overlaySettings_Stub.SetupGet(os => os.HarringtonMPositions).Returns(harringtonMPositions);
                _overlaySettings_Stub.SetupGet(os => os.HoleCardsPositions).Returns(holeCardsPositions);
            };

            Because of = () => _sut.InitializeWith(_overlaySettings_Stub.Object, seat);

            It should_initialize_the_PlayerStatus_viewmodel_with_the_harringtonM_and_holecards_positions_of_seat_1
                = () => _playerStatusVM_Mock.Verify(ps => ps.InitializeWith(holeCardsPositions[seat], harringtonMPositions[seat]));

        }

        [Subject(typeof(PlayerOverlayViewModel), "UpdateStatusWith")]
        public class when_updating_status_with_null_value : PlayerOverlayViewModelSpecs
        {
            Because of = () => _sut.UpdateStatusWith(null);

            It should_set_PlayerStatus_IsPresent_to_false = () => _playerStatusVM_Mock.VerifySet(ps => ps.IsPresent = false);
        }

        [Subject(typeof(PlayerOverlayViewModel), "UpdateStatusWith")]
        public class when_updating_with_converted_player_with_M_Greater_0 : Ctx_NonNull_ConvertedPlayer_Statistics_And_Setup_HarringtonM
        {
            const string playerName = "some name";

            const int M = 1;

            Establish context = () => {
                _convertedPlayer_Stub.SetupGet(cp => cp.Name).Returns(playerName);
                _convertedPlayer_Stub.SetupGet(cp => cp.MAfter).Returns(M);
            };

            Because of = () => _sut.UpdateStatusWith(_convertedPlayer_Stub.Object);

            It should_set_PlayerStatus_IsPresent_to_true = () => _playerStatusVM_Mock.VerifySet(ps => ps.IsPresent = true);

            It should_set_PlayerStatus_HarringtonM_Value_to_MAfter_of_converted_player = () => _harrintonM_VM_Stub.VerifySet(hm => hm.Value = M);

            It should_set_the_PlayerName_to_the_name_of_the_converted_player = () => _sut.PlayerName.ShouldEqual(playerName);
        }

        [Subject(typeof(PlayerOverlayViewModel), "UpdateStatusWith")]
        public class when_updating_with_converted_player_with_M_0_because_he_just_was_eliminated : Ctx_NonNull_ConvertedPlayer_Statistics_And_Setup_HarringtonM
        {
            const string playerName = "some name";

            const int M = 0;

            Establish context = () => {
                _convertedPlayer_Stub.SetupGet(cp => cp.Name).Returns(playerName);
                _convertedPlayer_Stub.SetupGet(cp => cp.MAfter).Returns(M);
            };

            Because of = () => _sut.UpdateStatusWith(_convertedPlayer_Stub.Object);

            It should_set_PlayerStatus_IsPresent_to_false = () => _playerStatusVM_Mock.VerifySet(ps => ps.IsPresent = false);

            It should_set_PlayerStatus_HarringtonM_Value_to_MAfter_of_converted_player = () => _harrintonM_VM_Stub.VerifySet(hm => hm.Value = M);

            It should_set_the_PlayerName_to_the_name_of_the_converted_player = () => _sut.PlayerName.ShouldEqual(playerName);
        }

        [Subject(typeof(PlayerOverlayViewModel), "UpdateStatusWith")]
        public class when_updating_with_converted_player_with_M_neg1_because_he_just_was_eliminated_and_calculations_were_inaccurate : Ctx_NonNull_ConvertedPlayer_Statistics_And_Setup_HarringtonM
        {
            const string playerName = "some name";

            const int M = -1;

            Establish context = () => {
                _convertedPlayer_Stub.SetupGet(cp => cp.Name).Returns(playerName);
                _convertedPlayer_Stub.SetupGet(cp => cp.MAfter).Returns(M);
            };

            Because of = () => _sut.UpdateStatusWith(_convertedPlayer_Stub.Object);

            It should_set_PlayerStatus_IsPresent_to_false = () => _playerStatusVM_Mock.VerifySet(ps => ps.IsPresent = false);

            It should_set_PlayerStatus_HarringtonM_Value_to_MAfter_of_converted_player = () => _harrintonM_VM_Stub.VerifySet(hm => hm.Value = M);

            It should_set_the_PlayerName_to_the_name_of_the_converted_player = () => _sut.PlayerName.ShouldEqual(playerName);
        }

        [Subject(typeof(PlayerOverlayViewModel), "UpdateStatisticsWith")]
        public class when_player_is_present_updating_statistics_with_null_value : PlayerOverlayViewModelSpecs
        {
            Establish context = () => _playerStatusVM_Mock.SetupGet(ps => ps.IsPresent).Returns(true);

            Because of = () => _sut.UpdateStatisticsWith(null);

            It should_set_IsPresentAndHasStatistics_to_false = () => _sut.IsPresentAndHasStatistics.ShouldBeFalse();
        }

        [Subject(typeof(PlayerOverlayViewModel), "UpdateStatisticsWith")]
        public class when_updating_with_non_null_statistics : Ctx_NonNull_ConvertedPlayer_Statistics_And_Setup_HarringtonM
        {
            Establish context = () => _playerStatusVM_Mock.SetupGet(ps => ps.IsPresent).Returns(true);

            Because of = () => _sut.UpdateStatisticsWith(_playerStatistics_Stub.Object);

            It should_assign_the_passed_PlayerStatistics_ViewModel = () => _sut.PlayerStatistics.ToString().ShouldEqual(_playerStatisticsVM_Stub.Object.ToString());

            It should_set_IsPresentAndHasStatistics_to_true = () => _sut.IsPresentAndHasStatistics.ShouldBeTrue();
        }

        [Subject(typeof(PlayerOverlayViewModel), "UpdateStatisticsWith")]
        public class when_updating_with_non_null_statistics_but_player_is_not_present : Ctx_NonNull_ConvertedPlayer_Statistics_And_Setup_HarringtonM
        {
            Establish context = () => _playerStatusVM_Mock.SetupGet(ps => ps.IsPresent).Returns(false);

            Because of = () => _sut.UpdateStatisticsWith(_playerStatistics_Stub.Object);

            It should_assign_the_passed_PlayerStatistics_ViewModel = () => _sut.PlayerStatistics.ToString().ShouldEqual(_playerStatisticsVM_Stub.Object.ToString());

            It should_set_IsPresentAndHasStatistics_to_false = () => _sut.IsPresentAndHasStatistics.ShouldBeFalse();
        }

        [Subject(typeof(PlayerOverlayViewModel), "ShowHoleCardsFor")]
        public class when_told_to_show_his_holecards_for_2_seconds_after_updated_with_non_null_converted_player_with_unknown_cards :
            Ctx_NonNull_ConvertedPlayer_Statistics_And_Setup_HarringtonM
        {
            const int duration = 2;

            const string holecards = "";

            Establish context = () => {
                _convertedPlayer_Stub.SetupGet(cp => cp.Holecards).Returns(holecards);
                _sut.UpdateStatusWith(_convertedPlayer_Stub.Object);
            };

            Because of = () => _sut.ShowHoleCardsFor(duration);

            It should_not_tell_the_PlayerStatus_to_show_the_holecards_for_2_seconds =
                () => _playerStatusVM_Mock.Verify(ps => ps.ShowHoleCardsFor(duration, holecards), Times.Never());
        }

        [Subject(typeof(PlayerOverlayViewModel), "ShowHoleCardsFor")]
        public class when_told_to_show_his_holecards_for_2_seconds_after_updated_with_non_null_converted_player_with_known_cards :
            Ctx_NonNull_ConvertedPlayer_Statistics_And_Setup_HarringtonM
        {
            const int duration = 2;

            const string holecards = "As Ks";

            Establish context = () => {
                _convertedPlayer_Stub.SetupGet(cp => cp.Holecards).Returns(holecards);
                _sut.UpdateStatusWith(_convertedPlayer_Stub.Object);
            };

            Because of = () => _sut.ShowHoleCardsFor(duration);

            It should_tell_the_PlayerStatus_to_show_the_holecards_for_2_seconds =
                () => _playerStatusVM_Mock.Verify(ps => ps.ShowHoleCardsFor(duration, holecards));
        }

        [Subject(typeof(PlayerOverlayViewModel), "ShowHoleCardsFor")]
        public class when_told_to_show_his_holecards_for_2_seconds_after_updated_with_null_converted_player : PlayerOverlayViewModelSpecs
        {
            const int duration = 2;

            Establish context = () => _sut.UpdateStatusWith(null);

            Because of = () => _sut.ShowHoleCardsFor(duration);

            It should_not_tell_the_PlayerStatus_to_show_the_holecards_for_2_seconds =
                () => _playerStatusVM_Mock.Verify(ps => ps.ShowHoleCardsFor(duration, Moq.It.IsAny<string>()), Times.Never());
        }

        [Subject(typeof(PlayerOverlayViewModel), "FilterAdjustmentRequested")]
        public class when_the_user_request_to_adjust_the_filter_for_the_players_statistics : PlayerOverlayViewModelSpecs
        {
            static bool filterAjustmentRequestedWasRaisedWithCorrectPayload;

            Establish context = () => {
                _sut.UpdateStatisticsWith(_playerStatisticsVM_Stub.Object);
                _sut.FilterAdjustmentRequested += arg => filterAjustmentRequestedWasRaisedWithCorrectPayload = arg.Equals(_playerStatisticsVM_Stub.Object);
            };

            Because of = () => _sut.FilterAdjustmentRequestedCommand.Execute(null);

            It should_let_me_know_and_pass_itself_as_the_payload = () => filterAjustmentRequestedWasRaisedWithCorrectPayload.ShouldBeTrue();
        }
    }
}