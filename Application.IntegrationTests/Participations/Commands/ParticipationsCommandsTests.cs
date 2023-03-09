namespace Tournament.Application.IntegrationTests.Participations.Commands;

using Tournament.Application.Options.Commands.SetAsAnswerOption;
using FluentAssertions;
using Tournament.Application.Common.Exceptions;
using Tournament.Application.Contests.Commands.CreateContest;
using Tournament.Application.Contests.Commands.DrawContest;
using Tournament.Application.Participations.Commands.CreateParticipation;
using Tournament.Application.Channels.Commands.CreateChannel;
using Tournament.Application.Questions.Commands.CreateQuestion;
using Tournament.Application.Options.Commands.CreateOption;
using Tournament.Application.Options.Commands.SetAsAnswerOption;
using Tournament.Domain.Entities;
using Tournament.Infrastructure.Persistence;
using static Testing;

public class CreateContestTests : BaseTestFixture
{

	[Test]
	public async Task ShouldRequireAllQs()
	{

		SeedData();
		var AccId="%$#33dfrrTEST";
		var Pcommand = new CreateParticipationCommand
		{
			ContestId = 1,
					  AccountId =AccId ,
					  Spent= 3.1415d,
					  OptionIds=new List<int>{1,6,7}
		}; 
		await FluentActions.Invoking(()=>SendAsync(Pcommand)).Should().ThrowAsync<ValidationException>();
	}

	[Test]
	public async Task ShouldRequireUniqueQs()
	{

		SeedData();
		var AccId="%$#33dfrrTEST";
		var Pcommand = new CreateParticipationCommand
		{
			ContestId = 1,
					  AccountId =AccId ,
					  Spent= 3.1415d,
					  OptionIds=new List<int>{1,2,6,7}
		}; 
		await FluentActions.Invoking(()=>SendAsync(Pcommand)).Should().ThrowAsync<ValidationException>();
	}

	[Test]
	public async Task ShouldRequireRelevantQs()
	{

		SeedData();
		var AccId="%$#33dfrrTEST";
		var Pcommand = new CreateParticipationCommand
		{
			ContestId = 1,
					  AccountId =AccId ,
					  Spent= 3.1415d,
					  OptionIds=new List<int>{1,3,6,7,13}
		}; 
		await FluentActions.Invoking(()=>SendAsync(Pcommand)).Should().ThrowAsync<ValidationException>();
	}

	[Test]
	public async Task ShouldNotParticipated()
	{

		SeedData();
		var AccId="dsf43#";
		var Pcommand = new CreateParticipationCommand
		{
			ContestId = 1,
					  AccountId =AccId ,
					  Spent= 3.1415d,
					  OptionIds=new List<int>{1,3,6,7}
		}; 
		//already participated
		await FluentActions.Invoking(()=>SendAsync(Pcommand)).Should().ThrowAsync<ValidationException>();
	}

	[Test]
	public async Task ShouldDrawEquallyCorrectly()
	{

		SeedData();
		var Pcommand1 = new CreateParticipationCommand
		{
			ContestId = 1,
					  AccountId ="#1" ,
					  Spent= 9d,
					  OptionIds=new List<int>{1,3,6,7}
		}; 
		var Pcommand2 = new CreateParticipationCommand
		{
			ContestId = 1,
					  AccountId ="#2" ,
					  Spent= 900d,
					  OptionIds=new List<int>{1,3,6,7}
		}; 
		var Pcommand3 = new CreateParticipationCommand
		{
			ContestId = 1,
					  AccountId ="#3" ,
					  Spent= 90000d,
					  OptionIds=new List<int>{1,3,6,7}
		}; 

		var pid1=await SendAsync(Pcommand1);
		var pid2=await SendAsync(Pcommand2);
		var pid3=await SendAsync(Pcommand3);

		var context = GetContext();
		var Total=context.Participations.Sum(a=>a.Spent);  
		var eqReward=Total/3;

		var Acommand1 = new SetAsAnswerOptionCommand
		{
			OptionId = 1,
					  QuestionId=1
		}; 
		var Acommand2 = new SetAsAnswerOptionCommand
		{
			OptionId = 3,
					  QuestionId=2
		}; 
		var Acommand3 = new SetAsAnswerOptionCommand
		{
			OptionId = 6,
					  QuestionId=3
		}; 
		var Acommand4 = new SetAsAnswerOptionCommand
		{
			OptionId = 7,
					  QuestionId=4
		}; 
		await SendAsync(Acommand1);
		await SendAsync(Acommand2);
		await SendAsync(Acommand3);
		await SendAsync(Acommand4);

		var contest = await FindAsync<Contest>(1);
		contest.Resolved.Should().Be(true);

		var Dcommand = new DrawContestCommand(1);
		await SendAsync(Dcommand);

		var p1 = await FindAsync<Participation>(pid1);
		var p2 = await FindAsync<Participation>(pid2);
		var p3 = await FindAsync<Participation>(pid3);
		p1.DrawnRank.Should().Be(3);
		p1.Reward.Should().BeApproximately(eqReward,1);
		p2.DrawnRank.Should().Be(2);
		p2.Reward.Should().BeApproximately(eqReward,1);
		p3.DrawnRank.Should().Be(1);
		p3.Reward.Should().BeApproximately(eqReward,1);
	}

	[Test]
	public async Task ShouldDrawWeightedRewardCorrectly()
	{

		SeedData();
		var context = GetContext();
		context.Contests.Find(1).WeightedReward=true;
		context.SaveChanges();
		var Pcommand1 = new CreateParticipationCommand
		{
			ContestId = 1,
					  AccountId ="#1" ,
					  Spent= 9d,
					  OptionIds=new List<int>{1,3,6,7}
		}; 
		var Pcommand2 = new CreateParticipationCommand
		{
			ContestId = 1,
					  AccountId ="#2" ,
					  Spent= 900d,
					  OptionIds=new List<int>{1,3,6,7}
		}; 
		var Pcommand3 = new CreateParticipationCommand
		{
			ContestId = 1,
					  AccountId ="#3" ,
					  Spent= 90000d,
					  OptionIds=new List<int>{1,3,6,7}
		}; 

		var pid1=await SendAsync(Pcommand1);
		var pid2=await SendAsync(Pcommand2);
		var pid3=await SendAsync(Pcommand3);


		var Acommand1 = new SetAsAnswerOptionCommand
		{
			OptionId = 1,
					  QuestionId=1
		}; 
		var Acommand2 = new SetAsAnswerOptionCommand
		{
			OptionId = 3,
					  QuestionId=2
		}; 
		var Acommand3 = new SetAsAnswerOptionCommand
		{
			OptionId = 6,
					  QuestionId=3
		}; 
		var Acommand4 = new SetAsAnswerOptionCommand
		{
			OptionId = 7,
					  QuestionId=4
		}; 
		await SendAsync(Acommand1);
		await SendAsync(Acommand2);
		await SendAsync(Acommand3);
		await SendAsync(Acommand4);

		var contest = await FindAsync<Contest>(1);
		contest.Resolved.Should().Be(true);

		var Dcommand = new DrawContestCommand(1);
		await SendAsync(Dcommand);

		var p1 = await FindAsync<Participation>(pid1);
		var p2 = await FindAsync<Participation>(pid2);
		var p3 = await FindAsync<Participation>(pid3);
		p1.DrawnRank.Should().Be(3);
		p1.Reward.Should().BeApproximately(9d,3);
		p2.DrawnRank.Should().Be(2);
		p2.Reward.Should().BeApproximately(900d,3);
		p3.DrawnRank.Should().Be(1);
		p3.Reward.Should().BeApproximately(90000d,3);
	}

	[Test]
	public async Task QuestionShouldBeResolvedByAnswer()
	{

		SeedData();
		var question = await FindAsync<Question>(1);
		question.Resolved.Should().Be(false);

		var Acommand1 = new SetAsAnswerOptionCommand
		{
			OptionId = 1,
					  QuestionId=1
		}; 
		await SendAsync(Acommand1);

		question = await FindAsync<Question>(1);
		question.Resolved.Should().Be(true);
	}

	[Test]
	public async Task ContestShouldBeResolvedByAnswers()
	{

		SeedData();
		var contest = await FindAsync<Contest>(1);
		contest.Resolved.Should().Be(false);

		var Acommand1 = new SetAsAnswerOptionCommand
		{
			OptionId = 1,
					  QuestionId=1
		}; 
		var Acommand2 = new SetAsAnswerOptionCommand
		{
			OptionId = 3,
					  QuestionId=2
		}; 
		var Acommand3 = new SetAsAnswerOptionCommand
		{
			OptionId = 6,
					  QuestionId=3
		}; 
		var Acommand4 = new SetAsAnswerOptionCommand
		{
			OptionId = 7,
					  QuestionId=4
		}; 
		await SendAsync(Acommand1);
		await SendAsync(Acommand2);
		await SendAsync(Acommand3);
		await SendAsync(Acommand4);

		contest = await FindAsync<Contest>(1);
		contest.Resolved.Should().Be(true);
	}
}
