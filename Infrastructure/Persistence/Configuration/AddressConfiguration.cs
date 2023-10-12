using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public static class AddressConfiguration
{
    public static void Configure(this EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(n => n.SId);
        builder.HasIndex(n => n.Id).IsUnique();

        builder.HasOne(n => n.Author)
            .WithOne(n => n.Address)
            .HasForeignKey<Address>(n => n.AuthorId)
            .HasPrincipalKey<Author>(n => n.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}