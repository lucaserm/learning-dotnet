using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Data;

public class BankDbContext : DbContext
{
    public BankDbContext(DbContextOptions<BankDbContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();
}