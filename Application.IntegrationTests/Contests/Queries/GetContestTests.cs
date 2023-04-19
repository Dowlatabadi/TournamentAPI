namespace Tournament.Application.IntegrationTests.Contests.Queries;

using FluentAssertions;
using Tournament.Application.Channels.Queries.GetChannel;
using Tournament.Application.Channels.Queries.GetChannels;
using Tournament.Application.Contests.Queries.GetContest;
using Tournament.Application.Contests.Queries.GetStat;
using Tournament.Application.Participations.Queries.GetContestParticipations;
using Tournament.Application.Questions.Queries.GetContestQuestions;
using Tournament.Application.Accounts.Queries.GetAccountParticipations;
using Tournament.Application.Options.Queries.GetQuestionOptions;

using static Testing;

public class GetContestTests : BaseTestFixture
{
	[Test]
	public async Task ShouldGetChannels()
	{
        var userId = await RunAsAdministratorAsync();
        SeedData();
		var query1 = new GetChannelsQuery();

		var result = await SendAsync(query1);

		result.Should().NotBeNull();
		result.Should().HaveCount(1);
	}

	[Test]
	public async Task ShouldGetChannel()
	{
        var userId = await RunAsAdministratorAsync();
        SeedData();
		var query1 = new GetChannelQuery(1);

		var result = await SendAsync(query1);

		result.Should().NotBeNull();
		result.Id.Should().Be(1);
	}

	[Test]
	public async Task ShouldGetParticipations()
	{
        var userId = await RunAsAdministratorAsync();
        SeedData();
		var query1 = new GetContestParticipationsQuery(1,1,100){
		};
		var result = await SendAsync(query1);

		result.Should().NotBeNull();
		result.Items.Should().HaveCount(2);
	}

	[Test]
	public async Task ShouldGetContest()
	{
        var userId = await RunAsAdministratorAsync();

        SeedData();
		var query1 = new GetContestQuery(1);

		var result = await SendAsync(query1);

		result.Should().NotBeNull();
		result.Id.Should().Be(1);
		result.ParticipationsTotalPoints.Should().Be(2.352);
		result.ParticipationsCount.Should().Be(2);
	}

	[Test]
	public async Task ShouldGetStat()
	{
        var userId = await RunAsAdministratorAsync();

        SeedData();
		var query1 = new GetContestStatQuery(1);

		var result = await SendAsync(query1);

		result[0].RewadsSpent = 2.352d;
		result[0].OptionsStats.FirstOrDefault(x=>x.OptionId==1).RewadsSpent.Should().Be(2.35d);
		result[0].OptionsStats.FirstOrDefault(x=>x.OptionId==2).RewadsSpent.Should().Be(.002d);
		result[0].OptionsStats.FirstOrDefault(x => x.OptionId == 2).Rate.Should().Be(2.352d/.002d);
		result[0].OptionsStats.FirstOrDefault(x => x.OptionId == 1).Rate.Should().Be(2.352d/2.35d);
		result[0].OptionsStats.FirstOrDefault(x => x.OptionId == 2).AnswersCount.Should().Be(1);

		result[1].OptionsStats.FirstOrDefault(x => x.OptionId == 3).AnswersCount.Should().Be(2);


		result.Should().NotBeNull();
		result.Should().HaveCount(4);
	}

	[Test]
	public async Task ShouldGetQuestions()
	{
        var userId = await RunAsAdministratorAsync();

        SeedData();
		var query1 = new GetContestQuestionsQuery(1);

		var result = await SendAsync(query1);

		result.Should().NotBeNull();
		result.Should().HaveCount(4);
		result.SelectMany(x=>x.Options).Should().HaveCount(7);
	}

	[Test]
	public async Task ShouldGetOptions()
	{
        var userId = await RunAsAdministratorAsync();

        SeedData();
		var query1 = new GetQuestionOptionsQuery(1);

		var result = await SendAsync(query1);

		result.Should().NotBeNull();
		result.Should().HaveCount(2);
	}

	[Test]
	public async Task ShouldGetAccountParticipations()
	{
        var userId = await RunAsAdministratorAsync();

        SeedData();
		var query1 = new GetAccountParticipationsQuery("dsf43#");

		var result = await SendAsync(query1);

		result.Should().NotBeNull();
		result.Should().HaveCount(1);
		result.SelectMany(x=>x.Answers).Should().HaveCount(4);
	}
}

