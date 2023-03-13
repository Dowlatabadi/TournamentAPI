
namespace Tournament.Application.Contests.Queries.GetContests
{
    public class ContestBriefDto
    {
        public int Id { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }

        public bool Resolved { get; set; }

        public bool IsActive { get; set; }
        public double Cost { get; set; }
        public int? Number { get; set; }
        public string guid { get; set; } = "";
        public double Reward { get; set; }

        public bool WeightedDraw { get; set; }

        public bool WeightedReward { get; set; }

        public int WinnersCapacity { get; set; }
        public int ParticipationCapacity { get; set; }

        public DateTime? Calculated { get; set; }

        public int ChannelId { get; set; }
        public string? ChannelTitle { get; set; }

        public int QuestionsCount { get; set; }
        public int ParticipationsCount { get; set; }
        public double ParticipationsTotalPoints { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? Finish { get; set; }


    }
}
