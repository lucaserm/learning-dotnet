using BankApi.Models;
using BankApi.Data;
using BankApi.Dtos;

using Microsoft.EntityFrameworkCore;
using BankApi.Exceptions;

namespace BankApi.Services;

public class AccountService : IAccountService
{
    private readonly BankDbContext _db;

    public AccountService(BankDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResponse<AccountResponse>> ListAsync(int page, int pageSize)
    {
        if (page < 0 || pageSize < 0 || pageSize > 100)
        {
            throw new BusinessRuleException(
                "page must be positive and pageSize must be between 0 and 100."
            );
        }

        var offset = page * pageSize;

        if (offset > int.MaxValue)
        {
            throw new BusinessRuleException("Page is too large.");
        }

        var query = _db.Accounts.AsNoTracking();

        var totalCount = await _db.Accounts.CountAsync();

        var items = await query
            .OrderBy(account => account.CreatedAt)
            .ThenBy(account => account.Id)
            .Skip(offset)
            .Take(pageSize)
            .Select(account => new AccountResponse(
                account.Id,
                account.Owner,
                account.Balance,
                account.CreatedAt
            ))
            .ToListAsync();

        return new PagedResponse<AccountResponse>(items, page, pageSize, totalCount);
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