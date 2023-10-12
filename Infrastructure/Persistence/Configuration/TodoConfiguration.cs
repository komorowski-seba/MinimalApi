using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public static class TodoConfiguration
{
    public static void Configure(this EntityTypeBuilder<Todo> builder)
    {
        builder.HasKey(n => n.SId);
        builder.HasIndex(n => n.Id).IsUnique();
        
        builder.Property(n => n.SId).ValueGeneratedOnAdd();

        builder.HasOne(n => n.Author)
            .WithMany(n => n.Todos)
            .HasForeignKey(n => n.AuthorId)
            .HasPrincipalKey(n => n.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}