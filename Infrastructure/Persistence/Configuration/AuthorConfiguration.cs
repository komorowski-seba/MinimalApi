using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public static class AuthorConfiguration
{
    public static void Configure(this EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(n => n.SId);
        builder.HasIndex(n => n.Id).IsUnique();

        builder.Property(n => n.SId).ValueGeneratedOnAdd();
        
        builder.Metadata
            .FindNavigation(nameof(Author.Todos))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}