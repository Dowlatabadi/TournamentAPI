using Tournament.Application.Common.Mappings;
using Tournament.Domain.Entities;

namespace Tournament.Application.Participations.Queries.GetContestParticipations
{
	public class ParticipationBriefDto: IMapFrom<Participation>
	{
		public int Id { get; set; }
		public string AccountId { get; set; }
		public int  ContestId { get; set; }
		public double  Spent { get; set; }
		public int  DrawnRank { get; set; }
		public double  Reward { get; set; }
		public DateTime Created {get; set;}
	}
}
