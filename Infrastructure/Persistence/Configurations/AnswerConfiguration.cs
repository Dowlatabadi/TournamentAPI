using Tournament.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tournament.Infrastructure.Persistence.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.HasIndex(x => new{x.OptionId,x.ParticipationId});
        builder.Property(t => t.ParticipationId)
            .IsRequired();
        builder.Property(t => t.OptionId)
          .IsRequired();
    }
}

