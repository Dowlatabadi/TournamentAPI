using Tournament.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tournament.Infrastructure.Persistence.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {

		builder.HasMany(x=>x.Options)
			.WithOne(x=>x.Question)
			.OnDelete(DeleteBehavior.Cascade);
    }
}

