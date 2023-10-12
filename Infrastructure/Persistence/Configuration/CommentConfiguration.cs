using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public static class CommentConfiguration
{
    public static void Configure(this EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(n => n.SId);
        builder.HasIndex(n => n.Id).IsUnique();
        
        builder.Property(n => n.SId).ValueGeneratedOnAdd();

        builder.HasOne(n => n.Todo)
            .WithMany(n => n.Comments)
            .HasForeignKey(n => n.TodoId)
            .HasPrincipalKey(n => n.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}