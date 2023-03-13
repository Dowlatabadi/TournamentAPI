using Tournament.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tournament.Infrastructure.Persistence.Configurations;

public class ContestConfiguration : IEntityTypeConfiguration<Contest>
{
    public void Configure(EntityTypeBuilder<Contest> builder)
    {
        builder.HasIndex(x => new { x.Title, x.Start, x.Finish, x.ChannelId ,x.Calculated, x.guid});
        builder.Property(t => t.Title)
            .HasMaxLength(300)
            .IsRequired();
    }
}

