namespace PokerTell.PokerHand.Tests
{
    using System;

    using Aquisition;

    using Infrastructure.Interfaces.PokerHand;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    internal class ThatAquiredPokerRound
    {
        AquiredPokerRound _aquiredRound;

        [SetUp]
        public void _Init()
        {
            _aquiredRound = new AquiredPokerRound();
        }

        [Test]
        public void ChipsGained_NoActionsAdded_ReturnsZero()
        {
            Assert.That(_aquiredRound.ChipsGained, Is.EqualTo(0));
        }

        [Test]
        public void ChipsGained_OneActionAdded_ReturnsChipsGainedByThatAction()
        {
            const double actionChipsGained = 1.0;
            var actionStub = new Mock<IAquiredPokerAction>();
            actionStub.SetupGet(get => get.ChipsGained).Returns(actionChipsGained);
            
            _aquiredRound.AddAction(actionStub.Object);

            Assert.That(_aquiredRound.ChipsGained, Is.EqualTo(actionChipsGained));
        }

        [Test]
        public void ChipsGained_TwoActionsAdded_ReturnsChipsGainedByBothActions()
        {
            const double firstActionChipsGained = 1.0;
            const double secondActionChipsGained = -0.5;
            const double expectedGain = firstActionChipsGained + secondActionChipsGained;
            
            var firstActionStub = new Mock<IAquiredPokerAction>();
            var secondActionStub = new Mock<IAquiredPokerAction>();
            firstActionStub.SetupGet(get => get.ChipsGained).Returns(firstActionChipsGained);
            secondActionStub.SetupGet(get => get.ChipsGained).Returns(secondActionChipsGained);

            _aquiredRound.AddAction(firstActionStub.Object);
            _aquiredRound.AddAction(secondActionStub.Object);

            Assert.That(_aquiredRound.ChipsGained, Is.EqualTo(expectedGain));
        }
    }
}