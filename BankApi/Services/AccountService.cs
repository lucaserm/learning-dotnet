using BankApi.Models;
using BankApi.Data;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Services;

public class AccountService : IAccountService
{
    private readonly BankDbContext _db;

    public AccountService(BankDbContext db)
    {
        _db = db;
    }

    public async Task<Account> CreateAsync(string owner)
    {
        var account = new Account(owner);

        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();

        return account;
    }

    public async Task<Account?> GetByIdAsync(Guid id)
    {
        return await _db.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(account => account.Id == id);
    }

    public async Task<Account?> DepositAsync(Guid id, decimal amount)
    {
        var account = await _db.Accounts.FindAsync(id);

        if (account is null)
        {
            return null;
        }

        account.Deposit(amount);
        await _db.SaveChangesAsync();

        return account;
    }

    public async Task<Account?> WithdrawAsync(Guid id, decimal amount)
    {
        var account = await _db.Accounts.FindAsync(id);

        if (account is null)
        {
            return null;
        }

        account.Withdraw(amount);
        await _db.SaveChangesAsync();

        return account;
    }
}