namespace Tournament.Application.IntegrationTests.Contests.Commands;

using Tournament.Application.Options.Commands.SetAsAnswerOption;
using FluentAssertions;
using Tournament.Application.Contests.Commands.CreateContest;
using Tournament.Application.Participations.Commands.CreateParticipation;
using Tournament.Application.Channels.Commands.CreateChannel;
using Tournament.Application.Questions.Commands.CreateQuestion;
using Tournament.Application.Options.Commands.CreateOption;
using Tournament.Domain.Entities;
using static Testing;

public class CreateContestTests : BaseTestFixture
{

    [Test]
    public async Task ShouldCreateContest()
    {

        var userId = await RunAsAdministratorAsync();
        var command1 = new CreateChannelCommand
        {
            Title = "ch test"
        };


        var ch_id = await SendAsync(command1);
        var command = new CreateContestCommand
        {
            Start = DateTime.Now,
            Finish = DateTime.Now.AddDays(2),
            CalculateOn = DateTime.Now.AddDays(2).AddSeconds(1),
            WeightedDraw = true,
            WeightedReward = false,
            Reward = 150d,
            WinnersCapacity = 3,
            ParticipationCapacity = 10,
            Title = "Contest test",
            ChannelId = ch_id
        };

        var id = await SendAsync(command);

        var contest = await FindAsync<Contest>(id);

        contest.Should().NotBeNull();
        contest!.WeightedDraw.Should().Be(true);
        contest!.WeightedReward.Should().Be(false);
        contest!.Reward.Should().Be(150d);
        contest!.WinnersCapacity.Should().Be(3);
        contest!.Resolved.Should().Be(false);

    }

    [Test]
    public async Task ShouldCreateQuestion()
    {
        var userId = await RunAsAdministratorAsync();
        SeedData();
        var Qcommand = new CreateQuestionCommand
        {
            Title = "Question test",
            ContestId = 1,
            Order = 1,
        };

        var qid = await SendAsync(Qcommand);
        var q = await FindAsync<Question>(qid);

        q.Should().NotBeNull();
        q!.Title.Should().Be(Qcommand.Title);
        //contest!.CreatedBy!.Should().Be(userId);
    }

    [Test]
    public async Task ShouldCreateOption()
    {
        var userId = await RunAsAdministratorAsync();
        SeedData();
        var Ocommand = new CreateOptionCommand
        {
            Title = "Question test",
            QuestionId = 1,
            Order=1,
        };

        var oid = await SendAsync(Ocommand);
        var o = await FindAsync<Option>(oid);

        o.Should().NotBeNull();
        o!.Title.Should().Be(Ocommand.Title);
        //contest!.CreatedBy!.Should().Be(userId);
    }

    [Test]
    public async Task ShouldCreateParticipation()
    {
        var userId = await RunAsAdministratorAsync();
        SeedData();
        var AccId = "%$#33dfrrTEST";
        var Pcommand = new CreateParticipationCommand
        {
            ContestId = 1,
            AccountId = AccId,
            Spent = 3.1415d,
            OptionIds = new List<int> { 1, 4, 6, 7 }
        };

        var pid = await SendAsync(Pcommand);
        var p = await FindAsync<Participation>(pid);

        var context = GetContext();
        var Answers = context.Answers.Where(a => a.ParticipationId == p.Id).ToList();

        p.Should().NotBeNull();
        p!.AccountId.Should().Be(AccId);
        Answers.Should().HaveCount(4);
        //contest!.CreatedBy!.Should().Be(userId);
    }

    [Test]
    public async Task ShouldSetOptionAsAnswer()
    {
        var userId = await RunAsAdministratorAsync();
        SeedData();
        var Scommand = new SetAsAnswerOptionCommand
        {
            QuestionId = 1,
            OptionId = 1
        };

        var res = await SendAsync(Scommand);
        var p = await FindAsync<Option>(1);
        var q = await FindAsync<Question>(1);

        p.Should().NotBeNull();
        p!.IsAnswer.Should().Be(true);
        q!.Resolved.Should().Be(true);
        //contest!.CreatedBy!.Should().Be(userId);
    }
}
