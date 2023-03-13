using Tournament.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tournament.Infrastructure.Persistence.Configurations;

public class ParticipationConfiguration : IEntityTypeConfiguration<Participation>
{
    public void Configure(EntityTypeBuilder<Participation> builder)
    {
        builder.HasIndex(x => new { x.AccountId,x.ContestId,x.DrawnRank });
        builder.Property(t => t.AccountId)
            .HasMaxLength(45)
            .IsRequired();
    }
}

