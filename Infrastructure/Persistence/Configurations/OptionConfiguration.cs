using Tournament.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tournament.Infrastructure.Persistence.Configurations;

public class OptionConfiguration : IEntityTypeConfiguration<Option>
{
    public void Configure(EntityTypeBuilder<Option> builder)
    {
        builder.HasIndex(x =>new { x.Title,x.QuestionId,x.Text });
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();
    }
}

