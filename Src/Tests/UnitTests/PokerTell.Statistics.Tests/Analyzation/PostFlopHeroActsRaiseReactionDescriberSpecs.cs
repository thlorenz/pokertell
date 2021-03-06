namespace PokerTell.Statistics.Tests.Analyzation
{
   using Infrastructure.Enumerations.PokerHand;
   using Infrastructure.Interfaces.PokerHand;

   using Interfaces;

   using Machine.Specifications;

   using Moq;

   using Statistics.Analyzation;

   using Tools.FunctionalCSharp;
   using Tools.Interfaces;

   using It = Machine.Specifications.It;

   // ReSharper disable InconsistentNaming

   public abstract class PostFlopHeroActsRaiseReactionDescriberSpecs
   {
      /* 
         *  Specification
         *  Subject is PostFlopHeroActsRaiseReactionDescriber
         *  
         *  Describe
         *  
         *      when fred  bet on flop out of position
                  � should contain "fred"  in description
                  � should contain "bet" in description
                  � should contain "flop" in description
                  � should contain "out of position" in description
                  � should contain and was raised in description

               when fred bet on turn in position
                  � should contain "fred"  in description
                  � should contain "bet" in description
                  � should contain " in position"  in description
                  � should contain " turn"  in description
                  � should contain and was raised in description

               when betsizes are 0 2 and 0 5
                  � should contain "0 2 to 0 5"  of the pot in description

               when betsizes are 0 2 and 0 2
                  � should contain "0 2 of the pot"  in description
                  � should not contain "0 2 to 0 2"  in description 
         */

      protected static Mock<IAnalyzablePokerPlayer> _analyzablePokerPlayerStub;

      protected static IPostFlopHeroActsRaiseReactionDescriber _sut;

      protected static ITuple<double, double> _validBetSizes;

      Establish context = () => {
         _validBetSizes = Tuple.New(0.5, 0.7);
         _analyzablePokerPlayerStub = new Mock<IAnalyzablePokerPlayer>();
         _sut = new PostFlopHeroActsRaiseReactionDescriber();
      };
   }

   public abstract class Ctx_PostFlopHeroActsRaiseReactionDescriber_ValidFlopAnalyzablePlayer
      : PostFlopHeroActsRaiseReactionDescriberSpecs
   {
      Establish context = () => {
         _analyzablePokerPlayerStub
            .SetupGet(p => p.ActionSequences)
            .Returns(new[] { ActionSequences.NonStandard, ActionSequences.HeroB });
         _analyzablePokerPlayerStub
            .SetupGet(p => p.InPosition)
            .Returns(new bool?[] { null, false });
      };
   }

    [Subject(typeof(PostFlopHeroActsRaiseReactionDescriber), "Description")]
   public class given_fred_bet_on_flop_out_of_position
      : PostFlopHeroActsRaiseReactionDescriberSpecs
   {
      static string description;

      Establish context = () => {
         _analyzablePokerPlayerStub
            .SetupGet(p => p.ActionSequences)
            .Returns(new[] { ActionSequences.NonStandard, ActionSequences.HeroB });
         _analyzablePokerPlayerStub
            .SetupGet(p => p.InPosition)
            .Returns(new bool?[] { null, false });
      };

      Because of =
         () => description = _sut.Describe("fred", _analyzablePokerPlayerStub.Object, Streets.Flop, _validBetSizes);

      It should_contain__bet__
         = () => description.ShouldContain("bet");

      It should_contain__flop__
         = () => description.ShouldContain("flop");

      It should_contain__fred___
         = () => description.Contains("fred").ShouldBeTrue();

      It should_contain__out_of_position__
         = () => description.ShouldContain("out of position");

      It should_contain_and_was_raised_
         = () => description.ShouldContain("and was raised");
   }


    [Subject(typeof(PostFlopHeroActsRaiseReactionDescriber), "Description")]
    public class given_fred_bet_on_turn_in_position
      : PostFlopHeroActsRaiseReactionDescriberSpecs
   {
      static string description;

      Establish context = () => {
         _analyzablePokerPlayerStub
            .SetupGet(p => p.ActionSequences)
            .Returns(new[] { ActionSequences.NonStandard, ActionSequences.NonStandard, ActionSequences.HeroB });
         _analyzablePokerPlayerStub
            .SetupGet(p => p.InPosition)
            .Returns(new bool?[] { null, null, true });
      };

      Because of =
         () => description = _sut.Describe("fred", _analyzablePokerPlayerStub.Object, Streets.Turn, _validBetSizes);

      It should_contain___in_position__
         = () => description.ShouldContain("in position");

      It should_contain___turn__
         = () => description.ShouldContain("turn");

      It should_contain__bet__
         = () => description.ShouldContain("bet");

      It should_contain__fred__
         = () => description.Contains("fred").ShouldBeTrue();

      It should_contain__and_was_raised__
         = () => description.ShouldContain("and was raised");
   }

    [Subject(typeof(PostFlopHeroActsRaiseReactionDescriber), "Description")]
   public class when_betsizes_are_0_2_and_0_5
      : Ctx_PostFlopHeroActsRaiseReactionDescriber_ValidFlopAnalyzablePlayer
   {
      static string description;

      Establish context = () => { _validBetSizes = Tuple.New(0.2, 0.5); };

      Because of =
         () => description = _sut.Describe("fred", _analyzablePokerPlayerStub.Object, Streets.Flop, _validBetSizes);

      It should_contain__0_2_to_0_5_of_the_pot__
         = () => description.ShouldContain("0.2 to 0.5 of the pot");
   }

    [Subject(typeof(PostFlopHeroActsRaiseReactionDescriber), "Description")]
   public class given_betsizes_are_0_2_and_0_2
      : Ctx_PostFlopHeroActsRaiseReactionDescriber_ValidFlopAnalyzablePlayer
   {
      static string description;

      Establish context = () => { _validBetSizes = Tuple.New(0.2, 0.2); };

      Because of =
         () => description = _sut.Describe("fred", _analyzablePokerPlayerStub.Object, Streets.Flop, _validBetSizes);

      It should_contain__0_2_of_the_pot__
         = () => description.ShouldContain("0.2 of the pot");

      It should_not_contain__0_2_to_0_2__
         = () => description.Contains("0.2 to 0.2 of the pot").ShouldBeFalse();
   }
}