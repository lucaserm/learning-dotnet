using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Data;

public class BankDbContext(DbContextOptions<BankDbContext> options) : DbContext(options)
{
  public DbSet<Account> Accounts => Set<Account>();
}