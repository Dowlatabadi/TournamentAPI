using Tournament.Domain.Entities;
using Tournament.Application.Common.Mappings;

namespace Tournament.Application.Channels.Queries.GetChannel
{
    public class ChannelDto: IMapFrom<Channel>
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }
    }
}
