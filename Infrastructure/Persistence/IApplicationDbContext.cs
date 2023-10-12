using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public interface IApplicationDbContext
{
   DbSet<Todo> Todos { get; }
   DbSet<Comment> Comments { get; }
   DbSet<Author> Authors { get; }
   DbSet<Address> Addresses { get; }

   Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}